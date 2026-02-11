# Team Task Manager (Console App)

A **C#/.NET console application** for managing team tasks. It supports creating and tracking tasks within a team, assigning tasks to users, managing priorities and statuses, detecting overdue tasks, and keeping task comments/history. The project was developed as part of a **Software Verification & Validation** course project.

## Features

- **Task CRUD**: create, view, update, delete tasks
- **Assign tasks** to team members (users)
- **Statuses**: `ToDo`, `InProgress`, `Testing`, `Done`
- **Priorities**: `Low`, `Medium`, `High`, `Critical`
- **Deadlines**: optional due dates + **overdue detection**
- **Advanced search & sorting**: filter by text/user/priority/deadline rules + sort by due date/priority/created date
- **Task comments/history**: add and list comments chronologically

## Tech Stack

- **C#** (.NET 6.0 or newer)
- Console UI (CLI)
- Testing:
  - **MSTest** unit tests
  - **Moq** for mocking repository dependencies
  - Data-driven tests: **[DataRow]**, **[DynamicData]** (including XML/CSV sources)

## Architecture

The solution is structured to be **testable** and easy to maintain, with clear separation of concerns:

- **Model layer**: core entities (`Task`, `User`, `Comment`, search options)
- **Repository layer**: in-memory repositories (e.g., `InMemoryTaskRepository`)
- **Service layer**:
  - `TaskService` for business logic and operations
  - `TaskAnalyzer` for analysis utilities (e.g., status evaluation, summaries)

## What is Covered

- **Service layer**
  - `TaskService`

- **In-memory repository**
  - `InMemoryTaskRepository`

- **Analytics helper**
  - `TaskAnalyzer`

- **Data-driven tests using**
  - `[DataRow]`
  - `[DynamicData]` with in-code collections
  - `[DynamicData]` with external XML and CSV test data files


---

## Quality, Metrics & Verification Highlights


In addition to unit testing and white-box analysis, the project included a comprehensive set of verification and validation activities covering inspection, functional testing, automation, API testing, load testing, and quality metrics evaluation.

A structured **code inspection process** was conducted using Azure DevOps. The inspected application was placed in a dedicated repository, and pull requests were created to simulate a formal review workflow. Comments and issue cards were created with clearly defined severity levels and references to predefined checklist items. A moderator coordinated the inspection process, distributed review responsibilities, and ensured structured evaluation. After completion, an inspection report was produced, including a Pareto analysis of identified defect causes and categories.

Comprehensive **unit testing** was performed for all implemented components (excluding the entry-point class), targeting at least 90% code coverage per class. Tests were written following best practices (independent, well-named, logic-free, properly structured) and included both standard and exception scenarios. Data-driven testing was implemented in multiple forms, and substitute (mock) objects were used to isolate unimplemented or external modules.

The project also included **Test-Driven Development (TDD)** for newly specified functionalities. Tests were written before implementation, and development was guided strictly by failing-to-passing test cycles. Documentation describes how new features evolved directly from test specifications.

For structural quality assessment, a method with higher cyclomatic complexity was selected for detailed analysis. The following activities were performed:

- McCabe metric calculation and control flow graph construction
- Comparison with automated Code Metrics tooling
- Full white-box testing using applicable structural testing techniques
- Code tuning using multiple optimization techniques with performance measurements (execution time and memory allocation across high iteration counts)
- Refactoring to improve maintainability index and complexity metrics, aligned with refactoring checklist principles

Additionally, the project included:

- Functional **black-box test case design**, documented with complete test scenario elements
- Automated UI testing using **Selenium IDE** and **NUnit Selenium framework**, applying **Page Object Model (POM)** and **Singleton pattern**
- **API testing** using Postman, covering all application functionalities
- **Load testing** using an industry-standard tool, followed by analysis of generated performance graphs and statistical metrics

All testing artifacts, reports, automation projects, and analysis results were documented and version-controlled within the project repository, ensuring full traceability between requirements, implementation, testing strategy, and quality outcomes.


## Team

- Emin Begić  
- Mirnes Fehrić  
- Zana Beljuri  
- Aldin Velić
