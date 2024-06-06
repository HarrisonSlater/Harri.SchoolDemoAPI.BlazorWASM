Feature: Navigation
Navigate using the nav side bar

Scenario: Navigate to the students page
	Given I am on the home page
	When I navigate to the students page
	Then I see a table full of students

Scenario: Navigate to the create new student page
	Given I am on the home page
	When I navigate to the create new student page
	Then I see the create new student form