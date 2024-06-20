# Csharp React Todo App

This app is a learning project to learn C# and .NET Core.

The app implements a REST API with a Postgres database, a C#/.NET Core backend and a React frontend.

The app is deployed on an AWS Ec2 instance. Occasionally AWS restarts the Ec2 instances. Please let me know if the app is down or if there are any problems with it. 

The app may be viewed at http://3.14.5.204:3000/todos

## What is C#?
C# is an object-oriented programming language developed by Microsoft. It was first released in 2000, designed by Anders Hejlsberg, and is intended to be simple, powerful, type-safe, and object-oriented. It is used for 
* web development, 
* desktop applications (especially for Windows), 
* mobile development, 
* game development (especially with the Unity game engine, which uses C# as its primary scripting language)
* cloud-based services.

## What are .NET, ASP.NET, .NET Core and ASP.NET Core?

* .NET Framework: The original, Windows-only framework for building various types of applications with extensive libraries and tools.
* ASP.NET: A framework within the .NET Framework (and .NET Core) for building web applications and services.
* .NET Core: A cross-platform, high-performance, modular, and open-source framework for building a wide range of applications.
* ASP.NET Core: The modern, cross-platform version of ASP.NET, designed to run on .NET Core, offering improved performance, modularity, and flexibility. 

## Problems / learnings I had developing the app.

The most significant problem I encountered developing the app was connecting the .NET Core backend with mysql on my AWS Ec2 instance. Due to time constraints, I decided to refactor the backend to work with postgres because, 1. there was no particular reason to run it on mysql as opposed to postgres, 2. I knew it was possible and straightforward to connect a backend to a postgres database on my Ec2 instance because I had done it before, and 3. the refactoring would be relatively simple. 

## Ideas for further development

1. Add a users table, so users can sign in and have their own todos saved
2. Add sessions.
3. Add ability to edit the todo (right now users can only mark the todos completed or not)
4. Make more responsive / mobile friendly
5. Add testing