Feature: Navigation History
The History of the browser should be set correctly

Scenario: View different pages of students and click the back button test
	Given I am on the home page
	When I navigate to the students page
	And I click next page
	And I click previous page
	And I click last page
	And I click first page

Scenario: View different pages of students and click the back button
	Given I am on the home page
	When I navigate to the students page
	And I click next page
	And I click previous page
	And I click last page
	And I click first page
	Then I see page 1 in the url
	When I click back
	Then I see the last page in the url
	When I click back
	Then I see page 1 in the url
	When I click back
	Then I see page 2 in the url
	When I click back 
	Then I see page 1 in the url
	When I click back
	Then I see the home url

Scenario: View different pages of students and create a student and click the back button
	Given I am on the home page
	When I navigate to the students page
	And I click next page
	And I navigate to the create new student page
	And I create a new student
	Then I should be on the students page
	When I click next page
	Then I see page 2 in the url
	And see a success alert for a new student
	When I click back
	Then I see page 1 in the url
	And see a success alert for a new student
	When I click back
	Then I should be on the create new student page
	When I click back
	Then I see page 2 in the url
	And should not see a success alert
	When I click back
	Then I see page 1 in the url
	And should not see a success alert
	When I click back
	Then I see the home url

Scenario: Edit student then go directly to create new student
	Given I am on the edit page for a student
	When I navigate to the create new student page
	Then I see the create new student form