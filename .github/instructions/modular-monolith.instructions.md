---
applyTo: "**"
---

## Solution Architecture

- I use clean architecture principles to structure my codebase.
- The codebase is divided into three main layers: Domain, Application, and Infrastructure.
- I skipped the presentation layer because in an api it mostly consists of endpoints. Endpoint I keep in my vertical slices.

## Domain Layer

- I follow DDD principles in the domain layer.
- For single value objects I use the packahge Vogen.
- For value objects with multiple properties I use records.
- For entites I use classes.
- Every Entity has an Id property. This property uses a value object as type. Under the hood this value object wraps a guid.
- I have a folder called Aggtegates inside the domain layer. This folder has nested folders for each aggregate root. Inside these folders I put the aggregate root entity, related entities and value objects.

## Application Layer

- The most important part of the application layer are the use cases.
- Each use case file contains multiple classes and records that work together to fullfil the use case. I also refer to these files as vertical slices.
- A use case consists of an endpoint class. This class inhertis from a base endpoint class that comes from the package FastEndpoints. There are multiple variants of this base class depending on the needs of the endpoint.
- Every use case is either a command or a query.
- A command use case has a record that implements ICommand from FastEndpoints. This record represents the request dto.
- A command use case also has a class that implements ICommandHandler<T> from FastEndpoints. This class contains the business logic for handling the command.
- A command use case also has a validator class that inherits from Validator<T> from FastEndpoints. This class contains the validation logic for the command request dto. The validator base class uses FluentValidation under the hood.
- NOTE: if you need to use scoped services inside the validator. You need to resolve them directly inside the rule. Check UpsertSingularHabit.ccs for an example.
- UpsertSingularHabit.cs is a perfect example of a command use case.
- A query use case also has a endpoint class that inherits from a FastEndpoints base endpoint class.
- A query use case has a record that is the request dto.
- A query use case handles the request inside the endpoint class directly. There is no separate handler class for queries.
- A query use case has a validator class that inherits from Validator<T> from FastEndpoints. This class contains the validation logic for the query request dto.
- GetDaySchedule.cs is a perfect example of a query use case.
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

## The Locked In Domain

- The Locked in domain is inspired by the book Atomic Habits by James Clear.
- The main aggregates is the DaySchedule aggregate.
- A DaySchedule represents a single week day for a user. It contains habits and routines that the user wants to do on that week day.
- A DaySchedule has a collection of Habits called SingularHabits. It also has a collection of Routines called Routines.
- Routines are a orderd sequence of habits you perform directly after each other. Atomic Habits refers to these as habit stacks. Singular Habits are habits that stand on their own.
- Each Habit has a Behaviour property. This property indicates what the habit is. For example: Drink water.
- Each Habit has a Cue property. This property indicates what the cue is to perform the habit. For example: At 12:00 at home. There are multiple cue types.
- Each Habit has a Identity property. This property indicates who you are when you perform the habit. For example: As a healthy person.
- The Cue.cs file shows how I handle polymorphism for the different cue types. MartenDb and swagger don't work well with polymorphism. So I use a wrapper record around all cue types. This record has a Type property that indicates the cue type. It also has a method to get the polymorphic cue object. It has type guards. And it has a factory method to make a wrapper around a polymorphic cue object.
- I want all my polymorphic value objects to have a structure like the Cue.cs file.
