---
applyTo: "**"
---

## Solution Architecture

- This solution follows the modular monolith architecture pattern.
- The Modules folder contains all the modules. Each module is an application that can become a microservice in the future if needed.
- Each module has its own bounded context.
- Each module consists of multiple dotnet projects (csproj).
- Each module follows clean architecture for code sructure.
- Each module is divided into three main layers: Domain, Application, and Infrastructure.
- I skipped the presentation layer because in an api it mostly consists of endpoints. Endpoints I keep in my vertical slices.

## Domain Layer

- I follow DDD principles in the domain layer.
- For single value objects I use the packahge Vogen.
- For value objects with multiple properties I use records.
- For entites I use classes.
- Every Entity has an Id property. This property uses a value object as type. Under the hood this value object wraps a guid.
- I have a folder called Aggtegates inside the domain layer. This folder has nested folders for each aggregate root. Inside these folders I put the aggregate root entity, related entities and value objects.
- Aggregate roots have properties with public getter and setters. Entities have internal setters. This way only the aggregate root can modify its related entities.
- Value objects that use records have init only properties. This way value objects are immutable.

## Application Layer

- The most important part of the application layer are the use cases.
- Each use case file contains multiple classes and records that work together to fullfil the use case. I also refer to these files as vertical slices.
- A use case consists of an endpoint class. This class inhertis from a base endpoint class that comes from the package FastEndpoints. There are multiple variants of this base class depending on the needs of the endpoint.
- Every use case is either a command or a query.
- A command use case has a record that implements ICommand from FastEndpoints. This record represents the request dto.
- A command use case also has a class that implements ICommandHandler<T> from FastEndpoints. This class contains the business logic for handling the command.
- A command use case also has a validator class that inherits from Validator<T> from FastEndpoints. This class contains the validation logic for the command request dto. The validator base class uses FluentValidation under the hood.
- NOTE: if you need to use scoped services inside the validator. You need to resolve them directly inside the rule. Check UpsertSingularHabit.ccs for an example.
- CreateCourse.cs is a perfect example of a command use case. This use case is part of the AcademicManagement module.
- A query use case also has a endpoint class that inherits from a FastEndpoints base endpoint class.
- A query use case has a record that is the request dto.
- A query use case handles the request inside the endpoint class directly. There is no separate handler class for queries.
- A query use case has a validator class that inherits from Validator<T> from FastEndpoints. This class contains the validation logic for the query request dto.
- GetCourses.cs is a perfect example of a query use case.
- The application layer also contains interfaces for repositories. These interfaces are implemented in the infrastructure layer.
- The application layer also contains interfaces for a unit of work. This interface is implemented in the infrastructure layer.
- The application layer also contains a Dtos folder. This folder contains dto records that are used in endpoint responses.
- I dont use dtos for all data in request and reponse objects. I only use dtos for entities. For value objects I use the value objects directly in request and response objects.
- The application layer also has a UsercContext service. This service provides information about the currently authenticated user.

## Infrastructure Layer

- The infrastructure layer contains implementations for repositories.
- The repositories all inherit from a generic repository base class.
- The infrastructure layer also contains a unit of work implementation. This repositories don't save anything until the unit of work's SaveChangesAsync method is called.
- The repositories and unit of work use MartenDb under the hood.
- I use martenDb as document database.

## The Academic Domain

- This solution has multiple modules. They are all about the academic domain.
- The project is an example to understand the modular monolith architecture pattern, clean architecture and DDD principles and vertical slices.
- The modules are:
  - AcademicManagement: Its used by presidents of universities and professors to manage courses.
  - StudentEnrollment: Its used by students to enroll in courses.

## Use Cases Per Module

- AcademicManagement module:

  - President use cases:
    - Create University (IMPLEMENTED)
    - Update University (IMPLEMENTED)
    - Archive University (IMPLEMENTED)
    - Create Department (IMPLEMENTED)
    - Update Department (IMPLEMENTED)
    - Archive Department (IMPLEMENTED)
    - Create Professor
    - Update Professor
    - Assign Professor to Department (as normal Professor or as Head of Department)
  - Professor use cases (As head of department for their own department):
    - UpsertCourse
    - ArchiveCourse
    - AssignProfessorToCourse (as normal professor or course owner)
  - Professor use cases (As Course owner for their own courses):
    - AddProfessorsToCourse
    - RemoveProfessorsFromCourse
    - CreateSection
    - AssignSectionProfessor
  - Professor use cases (as only professor of a section of a course):

    - AddAssignment
    - AddExam
    - UpdateSection
    - AddExamResult
    - AddAssignmentResult

  - OfficalAcademicRegistration module:

    - Register University
    - Show Universities
    - Register Student
    - Create Student Join University Request
    - Approve Student Join University Request

  - StudentEnrollment module:
    - Student use cases:
      - EnrollInCourse
      - SubmitAssignment
      - ViewGrades
      - WithdrawFromCourse
