# Reserved.com – Registration & Login
**Manual Test Flows and Test Cases (Task 1)**  
*Version:* 1.0 · *Date:* 2025-08-31  
*Scope:* Registration and Login journeys for **https://www.reserved.com/gb/en/**  
*Source of truth for detailed steps:* `Registration.feature`, `Login.feature` (Gherkin)

---

## 1) Audience & Goals
This document is written so both **non‑technical stakeholders** and **developers/testers** can understand what is covered and how to execute tests.  
It summarizes the **flows** and lists **up to 10 manual test cases**, each with clear preconditions, steps, and expected results.  
(Automation is addressed separately in Task 2.)

---

## 2) Assumptions & Test Environment
- Tester can access **https://www.reserved.com/gb/en/** on desktop web.
- If a **cookie consent** banner appears, it must be accepted to proceed (handled as Background in features).
- Email delivery is verifiable using a test mailbox (e.g., QA mailbox, MailHog, or equivalent).
- For social login (Google), a **pre‑linked account** is available for testing.
- No CAPTCHA or rate‑limiting prevents running these scenarios on the test environment.
- Password policy requires a **“strong”** password (examples used in features: `StrongP@ssw0rd!`, etc.).

---

## 3) Test Data (placeholders)
Use unique, realistic emails to avoid collisions (e.g., `user+timestamp@example.com`). Some examples used in features:
- **Valid registration examples:** `test.user123@example.com`, `test.user124@example.com`
- **Existing account:** `existing.user@example.com` (pre‑created)
- **Valid login:** `valid.user@example.com` (pre‑created)
- **Google‑linked account:** `user.google@example.com` (pre‑linked)
- **Edge case tokens in features:** `[spaces]` (whitespace only), `[256A]` (256 characters “A”)

> Exact addresses can be adjusted to your environment (e.g., your catch‑all domain).

---

## 4) High‑level Flows (from feature files)
**Flow A – Registration**  
1. Open the Reserved home page.  
2. Accept cookies (if prompted).  
3. Go to Registration.  
4. Fill required fields (email, name, surname, password).  
5. Accept Privacy Policy (mandatory) and optionally Newsletter.  
6. Submit and expect successful account creation + welcome email.

**Flow B – Login (email/password)**  
1. Open the Reserved home page.  
2. Accept cookies (if prompted).  
3. Go to Login.  
4. Enter email and password.  
5. Submit and expect successful sign‑in **or** a clear authentication/validation error.

**Flow C – Password Reset**  
1. From Login, choose **password recovery**.  
2. Request reset for a known email.  
3. Receive email, open reset link, set new password.  
4. Log in with the new password.

**Flow D – Google Sign‑in (linked account)**  
1. From Login, choose **Continue with Google**.  
2. Authenticate on Google with a **linked** email.  
3. Expect successful sign‑in on Reserved.

> Flows A–D are modeled in `Registration.feature` and `Login.feature` (Background + Scenarios).

---

## 5) Test Case Catalog (max 10)
IDs are stable labels that you can reuse in Jira/Xray/Allure.

### REG‑001 – Successful registration with valid data
**Objective:** A new user can register using valid details, accept privacy policy (mandatory), optionally opt‑in to newsletter.  
**Preconditions:** Test email not used before.  
**Steps:**  
1. Navigate Home → accept cookies if shown.  
2. Go to **Registration**.  
3. Fill: email, name, surname, **strong** password.  
4. Tick **Privacy Policy**; (optionally) tick **Newsletter**.  
5. Submit.  
**Expected:**  
- User is **logged in** after registration.  
- A **welcome email** is sent to the provided address.

*Traceability:* `Registration.feature` – *Scenario Outline: Successful registration with valid data* (`@smoke @regression`).

---

### REG‑002 – Registration blocked for an email that already has an account
**Objective:** Prevent duplicate account creation.  
**Preconditions:** `existing.user@example.com` already registered.  
**Steps:**  
1. Home → accept cookies.  
2. Registration → try to register with `existing.user@example.com` and valid other fields.  
3. Submit.  
**Expected:**  
- Clear error **“email is already in use”** (or equivalent).  
- Stay on registration page; **not** logged in; **no** welcome email.

*Traceability:* `Registration.feature` – *Scenario: Registration blocked for an email that already has an account* (`@negative @regression`).

---

### REG‑003 – Registration input validation (required/format/length/charset)
**Objective:** Field‑level validation is enforced for **email / name / surname** (blank, whitespace‑only, invalid characters, too long, invalid email).  
**Preconditions:** None.  
**Steps (examples):**  
- Leave **email** blank → submit.  
- Use invalid email format/domain (e.g., `user-at-example.com`, `user@invalid`) → submit.  
- Leave **name** or **surname** blank / whitespace only (`[spaces]`) → submit.  
- Enter invalid chars (`J@n3`, `D0e#`) or too long values (`[256A]`) → submit.  
**Expected:**  
- Inline validation messages for the specific field(s).  
- Account **not** created; **no** email sent.

*Traceability:* `Registration.feature` – *Scenario Outline: Registration form input validation* (`@negative @validation @regression`).

---

### REG‑004 – Registration password policy
**Objective:** Enforce password strength policy (length, required character classes, etc.).  
**Preconditions:** None.  
**Steps:**  
1. Registration → enter **weak** passwords that violate policy (e.g., too short, only letters, missing special).  
2. Submit.  
**Expected:**  
- See validation error describing the violation.  
- Account **not** created.

*Traceability:* `Registration.feature` – password policy checks included under validation outline (`@validation`).

---

### REG‑005 – Privacy policy mandatory; newsletter optional
**Objective:** Ensure Privacy Policy must be accepted; Newsletter is optional.  
**Preconditions:** None.  
**Steps:**  
1. Registration → leave **Privacy Policy** unchecked; submit.  
2. Registration → check **Privacy Policy**, uncheck **Newsletter**; submit with valid data.  
**Expected:**  
- (1) Error that privacy policy must be accepted; no account created.  
- (2) Successful registration without newsletter subscription.

*Traceability:* `Registration.feature` – examples include `privacy.notAccepted` and newsletter variants.

---

### LOG‑001 – Successful login with valid credentials
**Objective:** Existing user can sign in with correct email/password.  
**Preconditions:** `valid.user@example.com` exists with known password.  
**Steps:**  
1. Home → accept cookies.  
2. Go to **Login**.  
3. Enter valid email and password; submit.  
**Expected:**  
- User is **logged in** (account dashboard visible).

*Traceability:* `Login.feature` – *Scenario Outline: Successful login with valid credentials* (`@smoke @regression`).

---

### LOG‑002 – Login fails for wrong password
**Objective:** Authentication with wrong password is rejected.  
**Preconditions:** Valid account exists.  
**Steps:**  
1. Login → enter valid email + **wrong** password; submit.  
**Expected:**  
- Clear **authentication error**; remain on login page; **not** logged in.

*Traceability:* `Login.feature` – wrong password scenario (`@negative @regression`).

---

### LOG‑003 – Login form input validation
**Objective:** Validate presence/format for email and presence for password.  
**Preconditions:** None.  
**Steps (examples):**  
- Email **blank** + valid password → submit.  
- Email **invalid format/domain** → submit.  
- Password **blank** with valid email → submit.  
- **Both blank** → submit.  
**Expected:**  
- Field‑level validation messages; stay on login page; **not** logged in.

*Traceability:* `Login.feature` – *Scenario Outline: Login input validation* (`@negative @validation @regression`).

---

### LOG‑004 – Password reset and login with new password
**Objective:** User can recover access using password reset link.  
**Preconditions:** Email inbox accessible for the test user.  
**Steps:**  
1. Login → choose **password recovery**.  
2. Request reset for `<email>`; verify **reset email sent**.  
3. Open **reset link** from email; set `<newPassword>`.  
4. Return to Login; sign in with `<email>` / `<newPassword>`.  
**Expected:**  
- Success message after reset; **logged in** with new password.

*Traceability:* `Login.feature` – *Scenario Outline: User resets password and signs in with the new password* (`@recovery @smoke @regression`).

---

### LOG‑005 – Successful login with Google (linked account)
**Objective:** User with an account **linked to Google** can authenticate via Google SSO.  
**Preconditions:** Google‑linked Reserved account exists for `<email>`.  
**Steps:**  
1. Login → **Continue with Google**.  
2. Authenticate on Google as `<email>` (supply Google password when prompted).  
**Expected:**  
- User is **logged in** to Reserved.

*Traceability:* `Login.feature` – *Scenario Outline: Successful login with Google* (`@regression`).

---

## 6) References (in repo)
- `features/Registration.feature` – Registration: happy path, duplicate email, field validations, password policy, privacy/newsletter.  
- `features/Login.feature` – Login: email/password success, wrong password, input validation, password reset, Google SSO.

> These features already include **Background** steps to open the home page and accept cookies when shown.

---

## 7) Notes for Execution
- Prefer **fresh, unique emails** for registration tests to avoid collisions.  
- For email‑driven checks (welcome or reset emails), use the environment’s **test mailbox** and record message timestamps/subjects.  
- When social login prompts for cross‑account consent or 2FA, document the behavior and align with environment settings.

---

## 8) Glossary
- **Background (Gherkin):** Steps run before each scenario (e.g., open site, accept cookies).  
- **Scenario Outline:** Parameterized scenario executed for each row in **Examples**.  
- **Validation vs Authentication:** *Validation* checks input format/required fields; *Authentication* checks credentials.

---

*End of document.*


---

## 9) Task 2 – Automation candidates & approach for non‑automated cases

### 9.1 Which test cases I would automate (and why)
| ID      | Automate in CI? | Why automate / value | Notes |
|---------|------------------|----------------------|-------|
| **REG‑001** | **Yes (Smoke & Regression)** | Core happy path; high business impact; stable; excellent ROI. | Use unique emails; verify welcome email via IMAP if available. |
| **REG‑002** | **Yes** | Deterministic negative path; protects against regressions in duplicate checks. | Data‑driven with pre‑created account. |
| **REG‑003** | **Yes** | Field‑level validation is ideal for fast, data‑driven UI automation. | Cover edge inputs (blank/whitespace/invalid domain/length/charset). |
| **REG‑004** | **Yes** | Password policy is critical security validation and rarely changes; low flake risk. | Keep rules externalized for easy updates. |
| **REG‑005** | **Yes** | Mandatory checkbox (privacy) and optional newsletter are simple UI rules with high regression risk. | Assert both error state and success path. |
| **LOG‑001** | **Yes (Smoke)** | Mission‑critical path; must stay green; fast and stable. | Keep credentials in secrets vault. |
| **LOG‑002** | **Yes** | Negative auth is deterministic and quick; prevents confusing UX regressions. | Rate‑limit awareness if any. |
| **LOG‑003** | **Yes** | Input validation matrix fits data‑driven tests; low maintenance. | Reuse dataset from REG‑003 where possible. |
| **LOG‑004** | **Yes (Nightly)** | Valuable end‑to‑end coverage; validates email integration and reset flow. | Prefer nightly due to email latency; implement via IMAP (e.g., MailKit). |
| **LOG‑005** | **No (Manual / Ad‑hoc)** | Google SSO often involves external flows (consents, 2FA, captchas) → high flake risk in CI. | Cover via manual checks or stubbed provider in lower layers; optionally schedule a non‑blocking nightly canary on stable test Google account (no 2FA). |

**Summary:** Automate **9/10**. Keep **LOG‑005** manual (or limited canary) due to external SSO dependencies.

#### Recommended automation approach (tech‑agnostic)
- **Language/BDD:** C# (+ SpecFlow optional for step reuse and reporting).
- **UI framework:** Playwright for .NET (preferred for speed & reliability) or Selenium WebDriver if mandated.
- **Assertions:** FluentAssertions / NUnit/xUnit assertions.
- **Data:** Randomized but traceable test data; email aliases for uniqueness.
- **Secrets:** Store credentials and mailbox tokens in a secure secret store (e.g., user‑secrets/GitHub Actions secrets).

### 9.2 Handling of email‑dependent tests (REG‑001 welcome email, LOG‑004 reset)
- Use a dedicated **test mailbox** (catch‑all or alias) and fetch messages via **IMAP** (e.g., MailKit) within the test.
- Add a short **polling window** (e.g., up to 30–60s) for message arrival; extract links with robust selectors.
- Run these tests **nightly** or in a dedicated CI stage to avoid slowing the core smoke suite.

### 9.3 What to do with tests that are not automated
- **Manual execution** during release candidates and major UI updates, with a short checklist:
  - Confirm **visuals/labels**, **SSO button presence**, **consent screens**, **error copy**.
  - Verify **localization** (EN/GB strings), **accessibility** (keyboard focus/ARIA roles) when applicable.
- **Lower‑layer coverage** for SSO:
  - Use a **stubbed SSO provider** (if architecture allows) or contract tests against the identity layer to validate the integration without Google UI.
- **Exploratory sessions**:
  - Allocate time to probe uncommon edge cases (multi‑consent, blocked cookies, 3rd‑party cookie restrictions) that are brittle in automation.
- **Runbook/Evidence**:
  - Capture short screen recordings or screenshots + timestamps; log environment, user, and browser version for reproducibility.

### 9.4 Prioritization & CI strategy
- **PR/Commit (fast) suite:** LOG‑001, LOG‑002, LOG‑003, REG‑002, REG‑003, REG‑004, REG‑005 (≤ ~3–5 min).
- **Daily smoke (adds happy paths):** + REG‑001.
- **Nightly extended:** + LOG‑004 (email), any canary for LOG‑005 if feasible.
- **On‑demand exploratory:** Manual LOG‑005 with a checklist.

---

