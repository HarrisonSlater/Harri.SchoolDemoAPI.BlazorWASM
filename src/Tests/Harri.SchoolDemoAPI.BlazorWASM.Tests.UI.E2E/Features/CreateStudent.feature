@students
Feature: Create Student
Create new students

# Positive
@cleanupNewStudent
Scenario: Create new student with name
	Given I am on the create new student page
	When I enter a student name "Tester StudentE2E-2"
	And click save
	Then I should be redirected to the students page
	And see a success alert for a new student
	When I search for student using the success alert ID
	Then I should see the new student with name "Tester StudentE2E-2"

@cleanupNewStudent
Scenario: Create new student with name and GPA
	Given I am on the create new student page
	When I enter a student name "Tester StudentE2E-2"
	And I enter a student GPA "3.91"
	And click save
	Then I should be redirected to the students page
	And see a success alert for a new student
	When I search for student using the success alert ID
	Then I should see the new student with name "Tester StudentE2E-2" and GPA "3.91"

Scenario: Create new student with name and GPA without saving
	Given I am on the create new student page
	When I enter a student name "Tester StudentE2E-2 Not Saved"
	And I enter a student GPA "0.59"
	And navigate to the students page
	Then I should not see a success alert
	When I search for student "Tester StudentE2E-2 Not Saved"
	Then The students table should be empty

# Negative
Scenario: Enter an invalid student name when creating a new student
	Given I am on the create new student page
	When I enter a blank student name
	Then I should see a validation message for the Name
	And the save button should be disabled

Scenario: Enter an invalid student GPA creating a new student
	Given I am on the create new student page
	When I enter a blank student name
	And I enter a student GPA "1.111"
	And click save
	Then I should see a validation message for the Name and GPA