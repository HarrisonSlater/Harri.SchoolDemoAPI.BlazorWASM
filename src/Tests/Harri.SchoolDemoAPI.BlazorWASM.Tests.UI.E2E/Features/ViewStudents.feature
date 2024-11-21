@students
Feature: View Students
View all students and individual students, including filtering

Scenario: View different pages of students 
	Given I am on the students page
	Then I see a table full of students on page 1
	When I click next page
	Then I see a table full of students on page 2
	When I click previous page
	Then I see page 1 again
	When I click last page
	Then I see a table with at least one student
	When I click first page
	Then I see page 1 again

Scenario Outline: Go directly to a page of students
	Given I go directly to the students page <page>
	Then I should be on the students page
	And see page <page> in the url

	Examples: 
	| page |
	| 1    |
	| 6    |

# Filter scenarios
# Name
@cleanupNewStudent
Scenario: Filter students by exact name
	Given A new student with a unique name exists
	And I am on the students page
	And I see a table full of students
	When I search for the new student by name
	Then I should see only the new student

Scenario: Filter students by partial name
	Given I am on the students page
	And I see a table full of students
	When I search for student with name "Gonzalez"
	Then I should see only students with the name "Gonzalez"
	When I clear the student name filter
	Then I see page 1 again
	
Scenario: Enter a student name that does not match any students
	Given I am on the students page
	And I see a table full of students on page 1
	When I search for student with name "a8e37ebe-23c7-4485-ac38-2cf21079b36f"
	Then The students table should be empty
	When I clear the student name filter
	Then I see page 1 again

# SId
@cleanupNewStudent
Scenario: Filter students by exact id
	Given A new student "Test Student - Filtering ID" exists
	And I am on the students page
	And I see a table full of students
	When I search for the new student by id
	Then I should see only the new student

# filter by id partial
Scenario: Filter students by partial id
	Given I am on the students page
	And I see a table full of students
	When I search for student id "1"
	Then I should see only students containing the id "1"
	When I clear the student id filter
	Then I see page 1 again

# empty id results
Scenario: Enter a student ID that does not match any students
	Given I am on the students page
	And I see a table full of students on page 1
	When I search for student id "2147483647"
	Then The students table should be empty
	When I clear the student id filter
	Then I see page 1 again

#TODO Filter input errors
#Combo filter test
