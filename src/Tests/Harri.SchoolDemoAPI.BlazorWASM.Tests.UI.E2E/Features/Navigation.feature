Feature: Navigation
Navigate using the nav side bar

Scenario: Navigate to the students page
	Given I am on the home page
	When I navigate to the students page
	Then I see a table full of students

Scenario: Navigate to an out of bounds students page number
	Given I am on the home page
	When I go directly to the students page 100
	Then I see page 1 in the url
	And I should see an error alert

Scenario: Navigate to the create new student page
	Given I am on the home page
	When I navigate to the create new student page
	Then I see the create new student form

Scenario: Navigate to the edit student page
	Given I am on the students page
	When I click edit on the first student
	Then I should be on the edit student page