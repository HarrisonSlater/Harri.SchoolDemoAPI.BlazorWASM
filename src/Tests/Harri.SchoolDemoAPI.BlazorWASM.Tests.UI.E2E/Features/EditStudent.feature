@students @cleanupNewStudent
Feature: Edit Student
Edit individual students

# Positive
Scenario: Edit a student's name
	Given I am on the edit page for a new student "Tester Student 1 To Edit"
	When I enter a student name "Tester Student 1 Modified"
	And click save
	Then I should be redirected to the students page
	When I search for the updated student
	Then I should see the updated student with name "Tester Student 1 Modified"

Scenario: Edit a student's GPA
	Given I am on the edit page for a new student "Tester Student 2 To Edit GPA"
	When I enter a student name "Tester Student 2 To Edit GPA"
	And enter a student GPA "3.45"
	And click save
	Then I should be redirected to the students page
	When I search for the updated student
	Then I should see the updated student with name "Tester Student 2 To Edit GPA" and GPA "3.45"
	When I click edit on the first student

Scenario: Edit all student data
	Given I am on the edit page for a new student "Tester Student 3 To Edit GPA" with GPA "1.12"
	When I enter a student name "Tester Student 3 Modified"
	And enter a student GPA "2.99"
	And click save
	Then I should be redirected to the students page
	When I search for the updated student
	Then I should see the updated student with name "Tester Student 3 Modified" and GPA "2.99"
	When I click edit on the first student

Scenario: Edit all student data without saving
	Given I am on the edit page for a new student "Tester Student To Edit GPA" with GPA "1.12"
	When I enter a student name "Tester Student 4 Modified"
	And enter a student GPA "2.99"
	When I navigate to the students page
	And search for the updated student
	Then I should see the updated student with name "Tester Student To Edit GPA" and GPA "1.12"
	When I click edit on the first student

# Negative

Scenario: Enter all invalid student data when editing
	Given I am on the edit page for a student
	When I remove the student's name
	And enter a student GPA "2.999"
	And click save
	Then I should see a validation message for the Name and GPA