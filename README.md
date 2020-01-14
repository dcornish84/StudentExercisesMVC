# Student Exercise Web Application

## Displaying a List of Cohorts

Your first task for this exercise is to start a new Visual Studio Web Application (MVC) project named `StudentExercisesMVC`. Then make a controller and corresponding Razor templates in order to manage the cohorts for your database.

Use scaffolding to...

1. Create a `CohortsController` in your controllers directory.
1. Create a `Views > Cohorts` directory and use the scaffolding to the create the `Index`, `Details`, `Create`, `Edit`, and `Delete` views.
1. In your controller, use ADO.NET to execute SQL statements for all of those actions.

## Displaying a List of Instructors

Use scaffolding to...

1. Create an `InstructorsController` in your controllers directory.
1. Create a `Views > Instructors` directory and use the scaffolding to the create the `Index`, `Details`, `Create`, `Edit`, and `Delete` views.
1. In your controller, use ADO.NET to execute SQL statements for all of those actions.

When you create or edit an instructor, you should be able to assign the instructor to a cohort from a `select` element in the form.

## Displaying a List of Students

Use scaffolding to...

1. Create a `StudentsController` in your controllers directory.
1. Create a `Views > Students` directory and use the scaffolding to the create the `Index`, `Details`, `Create`, `Edit`, and `Delete` views.
1. In your controller, use ADO.NET to execute SQL statements for all of those actions.

When you create or edit a student, you should be able to assign the student to a cohort from a `select` element in the form.

## Displaying a List of Exercises

Use scaffolding to...

1. Create an `ExercisesController` in your controllers directory.
1. Create a `Views > Exercises` directory and use the scaffolding to the create the `Index`, `Details`, `Create`, `Edit`, and `Delete` views.
1. In your controller, use ADO.NET to execute SQL statements for all of those actions.

# Assigning Exercises to Students

## Multi-select for Many-to-Many

> **Note:** You will need a custom view model for this task _(e.g. `StudentEditViewModel`)_

Modify your student edit form to display all exercises in a multi-select element. The user should be able to select one, or more exercises, in that element. When the user submits the form, then the `StudentExercises` table in your database should have a new entry added for each of the exercises that were selected in the form.

## Details

When you view the details of an individual student, then there should be a list of assigned exercises in the view.


# Assigning Cohort to Instructor

## Editing an Instructor

> **Note:** You will need a custom view model for this task _(e.g. `InstructorEditViewModel`)_

Modify your Instructor edit form to display all cohorts in a select element. The user should be able to select one of the cohorts in the dropdown. When the user submits the form, then the corresponding row in the `Instructor` table in your database should have its `CohortId` column value updated.

## Details

When you view the details of an instructor, it should display the name of the cohort she is currently teaching.

# Assigning Cohort to Student

## Editing a Student's Cohort and Exercises

Take the work you did in the last exercise of assigning a cohort to an instructor and do the same for the student edit form. When you are done with this exercise, you should be able to successfully do the following actions when editing a student.

1. Assign a student to a single cohort
1. Assign 1-_n_ exercises to a student
