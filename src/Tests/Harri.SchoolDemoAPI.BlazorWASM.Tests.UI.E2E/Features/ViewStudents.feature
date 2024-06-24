@students
Feature: View Students
View all students and individual students

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


#Scenario Outline: Go directly to a page of students
#	Given I go directly to the students page <page>
#	Then I should be on the students page
#	And see page <page> in the url
#
#	Examples: 
#	| page |
#	| 1    |
#	| 6    |