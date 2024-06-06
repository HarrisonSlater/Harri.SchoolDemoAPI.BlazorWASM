@Students
Feature: Create Student
Create new students

# Positive
@cleanupNewStudent
Scenario: Create new student with name
	Given I am on the create new student page
	When I enter a student name Tester StudentE2E-2
	And click save
	Then I should be redirected back to the students page
	When I click last page
#	Then I should see the new student with name 'Tester StudentE2E'

@cleanupNewStudent
Scenario: Create new student with name and GPA
	Given I am on the create new student page
	When I enter a student name Tester StudentE2E-2
	And I enter a student GPA '3.91'
	And click save
	Then I should be redirected back to the students page
	When I click last page
	Then I should see the new student with name 'Tester StudentE2E-2' and GPA '3.91'

@cleanupNewStudent
Scenario: Create new student with name and GPA without saving
	Given I am on the create new student page
	When I enter a student name Tester StudentE2E-2
	And I enter a student GPA '0.59'
	And navigate to the students page
	When I click last page
	Then I should not see a student with name 'Tester StudentE2E-2' and GPA '0.59'

# Negative

Scenario: Enter an invalid student name when creating a new student
	Given I am on the create new student page
	When I remove the student's name
	And click save
	Then I should see a validation message

Scenario: Enter an invalid student gpa creating a new student
	Given I am on the create new student page
	When I change the GPA to '-1'
	And click save
	Then I should see a validation message

Scenario: Enter all invalid student data when creating a new student
	Given I am on the create new student page
	When I remove the student's name
	And change the GPA to '-1'
	And click save
	Then I should see a validation message