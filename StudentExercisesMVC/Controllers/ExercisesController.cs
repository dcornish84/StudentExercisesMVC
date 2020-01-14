using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using StudentExercisesMVC.Models;

namespace StudentExercisesMVC.Controllers
{
    public class ExercisesController : Controller
    {

        private readonly IConfiguration _config;

        public ExercisesController(IConfiguration config)
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

        // GET: Exercises
        public ActionResult Index()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, ExerciseName, ProgrammingLanguage FROM Exercise";

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
                    return View(exercises);
                }
            }


        }

        // GET: Exercises/Details/5
        public ActionResult Details(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, ExerciseName, ProgrammingLanguage FROM Exercise
                                        WHERE Id = @id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    var reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        var exercise = new Exercise
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            ExerciseName = reader.GetString(reader.GetOrdinal("ExerciseName")),
                            ProgrammingLanguage = reader.GetString(reader.GetOrdinal("ProgrammingLanguage"))
                        };

                        reader.Close();
                        return View(exercise);
                    }

                    reader.Close();
                    return NotFound();
                }
            }

        }

        // GET: Exercises/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Exercises/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Exercise exercise)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"INSERT INTO Exercise 
                                          (ExerciseName, ProgrammingLanguage)
                                          VALUES (@exerciseName, @programmingLanguage)";
                        cmd.Parameters.Add(new SqlParameter("@exerciseName", exercise.ExerciseName));
                        cmd.Parameters.Add(new SqlParameter("@programmingLanguage", exercise.ProgrammingLanguage));


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

        // GET: Exercises/Edit/5
        public ActionResult Edit(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, ExerciseName, ProgrammingLanguage
                                         FROM Exercise
                                         WHERE Id = @id";

                    cmd.Parameters.Add(new SqlParameter("Id", id));

                    var reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        var exercise = new Exercise
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            ExerciseName = reader.GetString(reader.GetOrdinal("ExerciseName")),
                            ProgrammingLanguage = reader.GetString(reader.GetOrdinal("ProgrammingLanguage"))

                        };

                        reader.Close();
                        return View(exercise);
                    }

                    reader.Close();
                    return NotFound();
                }
            }

        }

        // POST: Exercises/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Exercise exercise)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE Exercise
                                            SET ExerciseName = @exerciseName,
                                                ProgrammingLanguage = @programmingLanguage
                                            WHERE Id = @id";

                        cmd.Parameters.Add(new SqlParameter("@exerciseName", exercise.ExerciseName));
                        cmd.Parameters.Add(new SqlParameter("@programmingLanguage", exercise.ProgrammingLanguage));
                        cmd.Parameters.Add(new SqlParameter("@id", id));

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

        // GET: Exercises/Delete/5
        public ActionResult Delete(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT ExerciseName, ProgrammingLanguage, Id FROM Exercise WHERE Id = @id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    var reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        var exercise = new Exercise
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            ExerciseName = reader.GetString(reader.GetOrdinal("ExerciseName")),
                            ProgrammingLanguage = reader.GetString(reader.GetOrdinal("ProgrammingLanguage"))
                        };

                        reader.Close();
                        return View(exercise);
                    }

                    return NotFound();
                }
            }

        }

        // POST: Exercises/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
       
        public ActionResult Delete([FromRoute]int id, Exercise exercise)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"DELETE FROM Exercise WHERE Id = @id";

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
    }
}