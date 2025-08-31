Feature: Login

  Background:
    Given I am on the Reserved home page
    And I accept cookies if prompted

  @smoke @regression
  Scenario Outline: Successful login with valid credentials
    Given I have an existing account
    When I navigate to the login page
    And I sign in with:
      | email    | <email>    |
      | password | <password> |
    Then I should be logged in
    Examples:
      | email                    | password        |
      | valid.user@example.com   | StrongP@ssw0rd! |
      | another.user@example.com | S7rong!Pass     |

  @negative @regression
  Scenario: Login fails with an incorrect password
    Given I have an existing account
    When I navigate to the login page
    And I sign in with:
      | email    | valid.user@example.com |
      | password | WrongPass!1            |
    Then I should see an authentication error
    And I should remain on the login page
    And I should not be logged in

  @negative @regression
  Scenario: Login fails with an unregistered email
    When I navigate to the login page
    And I sign in with:
      | email    | not.registered@example.com |
      | password | StrongP@ssw0rd!            |
    Then I should see tha my account is not registered
    And I should remain on the login page
    And I should not be logged in

  @negative @validation @regression
  Scenario Outline: Login input validation
    When I navigate to the login page
    And I attempt to sign in with <case>:
      | email    | <email>    |
      | password | <password> |
    Then I should see a validation error for "<errorField>"
    And I should remain on the login page
    And I should not be logged in
    Examples:
      | case                | email               | password        | errorField |
      | email.blank         |                     | StrongP@ssw0rd! | email      |
      | email.invalidFormat | user-at-example.com | StrongP@ssw0rd! | email      |
      | email.invalidDomain | user@invalid        | StrongP@ssw0rd! | email      |
      | password.blank      | user+valid@test.com |                 | password   |
      | both.blank          |                     |                 | multiple   |


  @recovery @smoke @regression
  Scenario Outline: User resets password and signs in with the new password
    When I navigate to the login page
    And I choose password recovery
    And I request a password reset for "<email>"
    Then I should see a confirmation that a reset email has been sent
    And a password reset email should be sent to "<email>"

    When I open the reset password link from the email for "<email>"
    And I set a new password "<newPassword>"
    Then I should see a success message

    When I navigate to the login page
    And I sign in with email "<email>" and password "<newPassword>"
    Then I should be logged in

    Examples:
      | email                  | newPassword     |
      | valid.user@example.com | NewStr0ng!Pass1 |


  @social @google @onboarding
  Scenario Outline: First-time Google sign-in creates a new account
    Given I have an existing account linked with Google "<email>"
    When I navigate to the login page
    And I choose to continue with Google
    And I authenticate on Google as "<email>" with password "<password>"
    Then I should be logged in

    Examples:
      | email                   | password       |
      | user.google@example.com | G00glePass!234 |


