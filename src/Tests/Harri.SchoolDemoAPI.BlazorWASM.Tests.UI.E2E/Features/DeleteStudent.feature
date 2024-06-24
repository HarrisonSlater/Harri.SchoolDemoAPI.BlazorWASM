#@students
#Feature: DeleteStudent
#Delete individual students
#
#Scenario: Delete a student
#	Given I am on the edit page for a new student "Tester Student To Delete"
#	When I click delete
#	Then I should see the delete student dialog
#	When I click delete on the delete dialog
#	Then I should be redirected to the students page
#	And see a success alert for a deleted student
#	When I search for student "Tester Student To Delete"
#	Then The students table should be empty
#
#Scenario: Cancel deleting a student
#	Given I am on the edit page for a new student "Tester Student To Not Delete" with GPA "3.59"
#	When I click delete
#	Then I should see the delete student dialog
#	When I click cancel on the delete dialog
#	Then I should not see the delete student dialog
#	And I should be on the edit student page for an existing student with gpa
#	When I refresh the page
#	Then I should be on the edit student page for an existing student with gpa