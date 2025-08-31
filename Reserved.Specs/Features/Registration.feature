Feature: Registration

  Background:
    Given I am on the Reserved home page
    And I accept cookies if prompted

  @smoke @regression
  Scenario Outline: Successful registration with valid data
    When I navigate to the registration page
    And I register with:
      | email             | <email>      |
      | name              | <name>       |
      | surname           | <surname>    |
      | password          | <password>   |
      | newsletterConsent | <newsletter> |
      | privacy policy    | <policy>     |
    Then I should be logged in
    And a welcome email should be sent to <email>
    Examples:
      | email                    | name | surname | password        | newsletter | policy |
      | test.user123@example.com | Joe  | Jones   | StrongP@ssw0rd! | true       | true   |
      | test.user124@example.com | Anna | Nowak   | S7rong!Pass     | false      | true   |

  @negative @regression
  Scenario: Registration blocked for an email that already has an account
    Given I have an existing account
    When I navigate to the registration page
    And I attempt to register with:
      | email             | existing.user@example.com |
      | name              | Existing                  |
      | surname           | User                      |
      | password          | StrongP@ssw0rd!           |
      | newsletterConsent | true                      |
      | privacy policy    | true                      |
    Then I should see an error indicating the email is already in use
    And I should remain on the registration page
    And I should not be logged in
    And no welcome email should be sent

  @negative @validation @regression
  Scenario Outline: Password policy is enforced
    When I navigate to the registration page
    And I attempt to register with an invalid password (<case>):
      | email                 | user125@test.com    |
      | name                  | Jane                |
      | surname               | Doe                 |
      | password              | <password>          |
      | newsletterConsent     | <newsletterConsent> |
      | privacyPolicyAccepted | true                |
    Then I should see a password validation error
    And the account should not be created
    And no welcome email should be sent

    Examples:
      | case        | password | newsletterConsent |
      | tooShort    | Ab1!2    | true              |
      | noDigit     | Abcdefg! | true              |
      | noUppercase | abcdef1! | true              |
      | noLowercase | ABCDEF1! | true              |
      | noSymbol    | Abcdefg1 | true              |
      | tooShort    | Ab1!2    | false             |
      | noDigit     | Abcdefg! | false             |
      | noUppercase | abcdef1! | false             |
      | noLowercase | ABCDEF1! | false             |
      | noSymbol    | Abcdefg1 | false             |


  @negative @validation @regression
  Scenario Outline: Registration input validation (non-password fields)
    When I navigate to the registration page
    And I attempt to register with <case>:
      | email                 | <email>         |
      | name                  | <name>          |
      | surname               | <surname>       |
      | password              | StrongP@ssw0rd! |
      | newsletterConsent     | <newsletter>    |
      | privacyPolicyAccepted | <privacy>       |
    Then I should see a validation error for "<errorField>"
    And the account should not be created
    And no welcome email should be sent

    Examples:
      | case                      | email               | name     | surname  | newsletter | privacy | errorField            |
      | email.blank               |                     | Jane     | Doe      | false      | true    | email                 |
      | email.invalidFormat       | janedoe-at-test.com | Jane     | Doe      | false      | true    | email                 |
      | email.invalidDomain       | jane@invalid        | Jane     | Doe      | false      | true    | email                 |
      | name.blank                | user+valid@test.com |          | Doe      | false      | true    | name                  |
      | surname.blank             | user+valid@test.com | Jane     |          | false      | true    | surname               |
      | name.whitespaceOnly       | user+valid@test.com | [spaces] | Doe      | false      | true    | name                  |
      | surname.whitespaceOnly    | user+valid@test.com | Jane     | [spaces] | false      | true    | surname               |
      | name.invalidCharacters    | user+valid@test.com | J@n3     | Doe      | false      | true    | name                  |
      | surname.invalidCharacters | user+valid@test.com | Jane     | D0e#     | false      | true    | surname               |
      | name.tooLong              | user+valid@test.com | [256A]   | Doe      | false      | true    | name                  |
      | surname.tooLong           | user+valid@test.com | Jane     | [256A]   | false      | true    | surname               |
      | privacy.notAccepted       | user+valid@test.com | Jane     | Doe      | false      | false   | privacyPolicyAccepted |
      | all.blank                 |                     |          |          | false      | false   | multiple              |

    
  
