Feature: Home
Navigate using the Home page shortcut

Scenario: Click the View students shortcut
	Given I am on the home page
	When I click the view students shortcut
	Then I should be on the students page

Scenario: Click the Create new student shortcut
	Given I am on the home page
	When I click the create new student shortcut
	Then I should be on the create new student page
#
#@cleanupNewStudent
#Scenario: Click the Edit student shortcut
#	Given A new student "Tester Home Student" with GPA "1.99" exists
#	And I am on the home page
#	When I enter a student ID for an existing student
#	And click the edit student shortcut
#	Then I should be on the edit student page for an existing student with gpa
#
#Scenario: Click the Edit student shortcut for an ID that does not exist
#	Given I am on the home page
#	When I enter a student ID "9999999"
#	And click the edit student shortcut
#	Then I should be on the home page
#	And see an error alert for a non existing student "9999999"
#
#Scenario: Edit student shortcut input validation
#	Given I am on the home page
#	When I enter a student ID "asdf"
#	And click the edit student shortcut
#	Then I should see a validation error for the Student ID