# [.NET Trainee Test Task] File Uploader
## Description

This repository contains the Reenbit .NET Trainee Test Task. The solution has a Web application with a form, where the user can enter an email and .docx file and send this file to the Azure storage, and an Azure Function, which will send the email to the user with the link to the file secured by SAS token.

## Structure of the repository
This repository consists of five projects:

 1. ***Infrastructure*** - defines the set of services used by Web Application and Azure Function. Services used to provide access to the BLOB storage and send emails;
 2. ***WebApp*** - web application with a form where the user can upload a .docx file and add the user's email. UI is built on *Blazor*;
 3. ***AzureFunction*** - Azure function with BLOB storage trigger from already created BLOB and when the file is added to the blob the email is sent to the user with notification the file is successfully uploaded.
 4. ***Infrastructure.Tests*** - unit tests for the backend logic of the web application and the Azure function. Tests are written on the *xUnit* framework with the *Moq* framework for the mocking.
