@Students @createNewStudent @cleanupNewStudent
Feature: Edit Student
Edit individual students

Scenario: Navigate to the edit student page
	Given I am on the students page
	When I click edit on a student
	Then I see the edit student page

# Positive
Scenario: Edit a student's name
	Given I am on the edit page for a student
	When I change the student name
	And click save
	Then I should be redirected back to students
	And ?

Scenario: Edit a student's GPA
	Given I am on the edit page for a student
	When I change the student GPA
	And click save
	Then I should be redirected back to students
	And ?

Scenario: Edit all student data
	Given I am on the edit page for a student
	When I change the student GPA
	And the student name
	And click save
	Then I should be redirected back to students
	And ?

Scenario: Edit all student data without saving
	Given I am on the edit page for a student
	When I change the student GPA
	And the student name
	And navigate to the home page
	And navigate to the students page
	And find the student I clicked on before
	Then the student should not have changed
	And ?

# Negative

Scenario: Enter an invalid student name when editing
	Given I am on the edit page for a student
	When I remove the student's name
	And click save
	Then I should see a validation message

Scenario: Enter an invalid student gpa when editing
	Given I am on the edit page for a student
	When I change the GPA to '-1'
	And click save
	Then I should see a validation message

Scenario: Enter all invalid student data when editing
	Given I am on the edit page for a student
	When I remove the student's name
	And change the GPA to '-1'
	And click save
	Then I should see a validation message