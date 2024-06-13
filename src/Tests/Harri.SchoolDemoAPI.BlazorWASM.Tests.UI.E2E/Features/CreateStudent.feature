@students
Feature: Create Student
Create new students

# Positive
# TODO cleanup tag
@cleanupNewStudent
Scenario: Create new student with name
	Given I am on the create new student page
	When I enter a student name "Tester StudentE2E-2"
	And click save
	Then I should be redirected to the students page
	And see a success alert for a new student
	When I search for student using the success alert id
	Then I should see the correct student with name "Tester StudentE2E-2"

@cleanupNewStudent
Scenario: Create new student with name and GPA
	Given I am on the create new student page
	When I enter a student name "Tester StudentE2E-2"
	And I enter a student GPA "3.91"
	And click save
	Then I should be redirected to the students page
	And see a success alert for a new student
	When I search for student using the success alert id
	Then I should see the correct student with name "Tester StudentE2E-2" and GPA "3.91"

@cleanupNewStudent
Scenario: Create new student with name and GPA without saving
	Given I am on the create new student page
	When I enter a student name "Tester StudentE2E-2 Not Saved"
	And I enter a student GPA "0.59"
	And navigate to the students page
	Then I should not see a success alert
	When I click last page
	Then I should not see a student with name "Tester StudentE2E-2 Not Saved" and GPA "0.59"

# Negative

Scenario: Enter an invalid student name when creating a new student
	Given I am on the create new student page
	When I enter a blank student name
	And click save
	Then I should see a validation message for the Name

Scenario: Enter an invalid student gpa creating a new student
	Given I am on the create new student page
	When I enter a student GPA "1.111"
	And click save
	Then I should see a validation message for the Name and GPA

#Scenario: Enter all invalid student data when creating a new student
#	Given I am on the create new student page
#	When I remove the student's name
#	And change the GPA to 
#	And click save
#	Then I should see a validation message for the Name and GPA