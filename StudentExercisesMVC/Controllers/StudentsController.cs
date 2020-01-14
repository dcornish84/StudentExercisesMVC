using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using StudentExercisesMVC.Models;
using StudentExercisesMVC.Models.ViewModels;

namespace StudentExercisesMVC.Controllers
{
    public class StudentsController : Controller
    {

        private readonly IConfiguration _config;

        public StudentsController(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }


        // GET: Students
        public ActionResult Index()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, FirstName, LastName, SlackHandle, CohortId FROM Student";

                    var reader = cmd.ExecuteReader();

                    var students = new List<Student>();

                    while (reader.Read())
                    {
                        students.Add(new Student
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                            CohortId = reader.GetInt32(reader.GetOrdinal("CohortId"))

                        });
                    }

                    reader.Close();
                    return View(students);
                }
            }


        }

        // GET: Students/Details/5
        public ActionResult Details(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT s.Id, s.FirstName, s.LastName, s.SlackHandle, s.CohortId, c.CohortName, c.Id
                                        FROM Student s
                                        JOIN Cohort c ON s.CohortId = c.Id
                                        WHERE s.Id = @id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    var reader = cmd.ExecuteReader();

                    Student student = null;
                    while (reader.Read())
                    {
                        if (student == null)
                        {
                            student = new Student
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                                CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),
                                Cohort = new Cohort
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    CohortName = reader.GetString(reader.GetOrdinal("CohortName"))
                                }
                            };
                        }
                        student.Exercises = GetExercisesByStudentId(id);
                    }
                    reader.Close();
                    if (student == null)
                    {
                        return NotFound();
                    }
                    return View(student);
                }
            }

        }

        // GET: Students/Create
        public ActionResult Create()
        {
            var cohorts = GetCohorts().Select(d => new SelectListItem
            {
                Text = d.CohortName,
                Value = d.Id.ToString()
            }).ToList();

            var viewModel = new StudentViewModels
            {
                Student = new Student(),
                Cohorts = cohorts
            };

            return View(viewModel);
        }

        // POST: Students/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Student student)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"INSERT INTO Student
                                          (FirstName, LastName, SlackHandle, CohortId)
                                          VALUES (@firstName, @lastName, @slackHandle, @cohortId)";
                        cmd.Parameters.Add(new SqlParameter("@firstName", student.FirstName));
                        cmd.Parameters.Add(new SqlParameter("@lastName", student.LastName));
                        cmd.Parameters.Add(new SqlParameter("@slackHandle", student.SlackHandle));
                        cmd.Parameters.Add(new SqlParameter("@cohortId", student.CohortId));

                        cmd.ExecuteNonQuery();
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Students/Edit/5
        public ActionResult Edit(int id)
        {
            var cohorts = GetCohorts().Select(d => new SelectListItem
            {
                Text = d.CohortName,
                Value = d.Id.ToString()
            }).ToList();

            var exercises = GetAllExercises().Select(e => new SelectListItem
            {
                Text = e.ExerciseName,
                Value = e.Id.ToString()
            }).ToList();

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT s.Id, s.FirstName, s.LastName, s.SlackHandle, s.CohortId, c.CohortName FROM Student s 
                                        LEFT JOIN Cohort c ON s.CohortId = c.Id
                                        WHERE s.Id = @Id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    var reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        var student = new Student
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                            CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),
                            ExerciseIds = GetExercisesByStudentId(id).Select(e => e.Id).ToList()
                        };

                        reader.Close();

                        var viewModel = new StudentEditViewModel
                        {
                            Student = student,
                            Cohorts = cohorts,
                            Exercises = exercises
                        };

                        return View(viewModel);
                    }

                    reader.Close();
                    return NotFound();
                }
            }

        }

        // POST: Students/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Student student)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE Student 
                                            SET FirstName = @firstName,
                                                LastName = @lastName,
                                                SlackHandle = @slackHandle,
                                                CohortId = @cohortId
                                                WHERE Id = @id";

                        cmd.Parameters.Add(new SqlParameter("@firstName", student.FirstName));
                        cmd.Parameters.Add(new SqlParameter("@lastName", student.LastName));
                        cmd.Parameters.Add(new SqlParameter("@slackHandle", student.SlackHandle));
                        cmd.Parameters.Add(new SqlParameter("@cohortId", student.CohortId));
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        cmd.ExecuteNonQuery();
                    }
                }

                DeleteAssignedExercises(student.Id);
                AddStudentExercise(student.Id, student.ExerciseIds);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        // GET: Students/Delete/5
        public ActionResult Delete(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, FirstName, LastName, SlackHandle, CohortId FROM Student WHERE Id = @id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    var reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        var student = new Student
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                            CohortId = reader.GetInt32(reader.GetOrdinal("CohortId"))

                        };

                        reader.Close();
                        return View(student);
                    }

                    return NotFound();
                }
            }

        }

        // POST: Students/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete([FromRoute]int id, Student student)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"DELETE FROM Student WHERE Id = @id";

                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        cmd.ExecuteNonQuery();

                        return RedirectToAction(nameof(Index));

                    }
                }


            }
            catch
            {
                return View();
            }
        }



        //Helper Methods------------------------------------
        private List<Cohort> GetCohorts()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, CohortName From Cohort";

                    var reader = cmd.ExecuteReader();

                    var cohorts = new List<Cohort>();

                    while (reader.Read())
                    {
                        cohorts.Add(new Cohort
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            CohortName = reader.GetString(reader.GetOrdinal("CohortName"))
                        });
                    }

                    reader.Close();
                    return cohorts;
                }
            }
        }

        private List<Exercise> GetAllExercises()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, ExerciseName, ProgrammingLanguage 
                                       FROM Exercise";
                    var reader = cmd.ExecuteReader();
                    var exercises = new List<Exercise>();
                    while (reader.Read())
                    {
                        exercises.Add(new Exercise
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            ExerciseName = reader.GetString(reader.GetOrdinal("ExerciseName")),
                            ProgrammingLanguage = reader.GetString(reader.GetOrdinal("ProgrammingLanguage"))
                        });
                    }
                    reader.Close();
                    return exercises;
                }
            }
        }

            private List<Exercise> GetExercisesByStudentId(int studentId)
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"SELECT e.Id AS ExerciseId, e.ExerciseName, e.ProgrammingLanguage  FROM
                                        Exercise e INNER JOIN StudentExercises se ON e.Id = se.ExerciseId
                                        WHERE StudentId = @StudentId";
                        cmd.Parameters.AddWithValue("@StudentId", studentId);
                        var reader = cmd.ExecuteReader();
                        List<Exercise> exercises = new List<Exercise>();
                        while (reader.Read())
                        {
                            Exercise exercise = new Exercise
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("ExerciseId")),
                                ExerciseName = reader.GetString(reader.GetOrdinal("ExerciseName")),
                                ProgrammingLanguage = reader.GetString(reader.GetOrdinal("ProgrammingLanguage")),
                            };
                            exercises.Add(exercise);
                        };
                        reader.Close();
                        return exercises;
                    }
                }
            }

        private void AddStudentExercise(int studentId, List<int> exerciseIds)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                foreach (var exerciseId in exerciseIds)
                {
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"INSERT INTO StudentExercises(StudentId, ExerciseId) 
                                       VALUES(@StudentId, @ExerciseId)";
                        cmd.Parameters.AddWithValue("@StudentId", studentId);
                        cmd.Parameters.AddWithValue("@ExerciseId", exerciseId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        private void DeleteAssignedExercises(int studentId)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"DELETE FROM StudentExercises WHERE StudentId = @studentId";
                        cmd.Parameters.Add(new SqlParameter("@studentId", studentId));
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
                throw;
            }
        }

    }
}
