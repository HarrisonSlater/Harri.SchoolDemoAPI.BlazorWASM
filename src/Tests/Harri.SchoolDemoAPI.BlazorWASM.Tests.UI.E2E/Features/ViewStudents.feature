@Students @e2e
Feature: ViewStudents
View all students and individual students

Scenario: Navigate to students 
	Given I am on the home page
	When I navigate to the students page
	Then I should see a table full of students

# WIP
#Scenario: View different pages of students 
#	Given I am on the students page
#	Then I should see a table full of students on page 1
#	When I click next
#	Then I should see a table full of students on page 2
#	When I click previous
#	Then I should see page 1 again
#	When I click last page
#	Then I should see a table with atleast one student
#	When I click first page
#	Then I should see page 1 again

