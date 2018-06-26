# SSDT Data Migration
The SSDT Data Migration extension offers an advanced script based management for database scheme migrations. The main functionality is provided by a fully customizable and self-managed pre-, post and reference data script execution. 

## Intro
SSDT is a database development tooling fully integrated into Microsoft Visual Studio. It supports database scheme migrations out of the box using DACPAC definition files and consist of an integrated single pre- and post script file capability. In practice more complex environments and database deployment scenarios require additional logic which requires an extended migration management. 

The SSDT Data Migration Scripts extension provides additional logic and management to meet the complexity for database deployments.

### Database Deployment Flow
Based on the single pre-, post and reference data scripts the extension allows to use multiple managed sub-deployment scripts:
 - Pre-Scripts
     - ...must be used with caution as the target database is in an undefined state as long as the actual scheme update has not been executed. Especially, if the database does not yet exist on the target server there is no way to run any of the pre-deployment scripts. must be used with 
 - Post-Scripts
     - ...are less critical compared to pre-scripts since the scheme migration has been executed in advance and provides an upgraded and consistent target database scheme.
 - Reference Data Scripts
     - ...are used for data which is needed by the application logic (extendable business logic, configuration, translations, etc...). Reference data should always be consistent with the application binaries. Most often, reference data uses merge statements for static reference table contents.

The following illustration shows the basic deployment flow using SSDT and the script extensibility features. All scripts and scheme migrations are bundled as artifacts during the build process and published onto the database by the dacfx framework at deployment time. 

![alt text](Docs/Images/DeploymentProcessFlow.png "Deployment PRocess Flow")

## Getting started
These instructions will get you a sample of the SSDT project up and running on your local machine for development and testing purposes.

### Prerequisites
This chapter lists all prerequisites which have to be met to run the extension locally or on build servers.
 - Install **SQL Server Data Tools**  Visual Studio component (*Visual Studio Installer | Modify | Individual Components | Cloud, database and server | SQL Server Data Tools*)
 - Install **4tecture.CustomSSDTMigrationScripts.msi** extension from the releases section.


### Setup Sample
Use the sampels project from the *Sample* folder as the initial template project or follow these instructions (using default configuration settings):
1. Create a new SQL Server Database project (*New Project... | Other Languages | SDQL Server*)
2. On the root level of the project create the following folder structure and files with the given build option:
    ```
    |-Scripts
        |-PostScripts (folder)
            |-{yourPostScript1.sql} (Build Action = None)
            |-...
        |-PreScripts (folder)
            |-{yourPreScript1.sql} (Build Action = None)
            |-...    
        |-ReferenceDataScripts (folder)
            |-{yourPostScript1.sql} (Build Action = None)
            |-...
        |-Script.PostDeployment.sql (Build Action = PostDeploy)
        |-Script.PreDeployment.sql (Build Action = PreDeploy)
    |-Tables
        |-_MigrationScriptsHistory.sql (Build Action = Build)
    ```
    - The migration scripts history table (*_MigrationScriptsHistory.sql*) is mandatory and must be setup with the following data definition language template:
        ```sql
        CREATE TABLE [dbo].[_MigrationScriptsHistory]
        (
            [ScriptNameId] NVARCHAR(255) NOT NULL PRIMARY KEY, 
            [ExecutionDate] DATETIME2 NOT NULL, 
            [ScriptHash] NVARCHAR(255) NOT NULL
        )

        ```
    - The SQL pre-script (*Script.PreDeployment.sql*) must be setup as follow:
        ```sql
        :r .\RunPreScriptsGenerated.sql
        ``` 
    - The SQL post.script (*Script.PostDeployment.sql*) must be initialized as followed:
        ```sql
        :r .\RunReferenceDataScriptsGenerated.sql
        :r .\RunPostScriptsGenerated.sql
        ```

4. Open the .sqlproj project file in a text editor and add the following project import statement .props within the projects root element:

    ```xml
    <Project DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003" ToolsVersion="4.0">

    <Import Project="$(MSBuildExtensionsPath)\4tecture\CustomSSDTMigrationScripts.props" />
        
    </Project>
    ```  
5. Rebuild your SSDT project and verify if the following files have been generated within the Script subfolder:

    - RunPostScriptsGenerated.sql
    - RunPreScriptsGenerated.sql

## Configuration
The extension can be easily adapted to your needs by using a json configuration file. The configuration file must be placed beside the SSDT project file named **ssdt.migration.scripts.json** The following snipped shows all available options where each of the supported script type can be individually configured.
```json
{
  "PreScripts": {
    "ScriptBaseDirectory": "<Value>",
    "ScriptNamePattern": "<Value>",
    "ScriptRecursiveSearch": "<Value>",
    "GeneratedScriptPath": "<Value>",
    "ExecutionFilterMode": "<Value>",
    "ExecutionFilterValue": "<Value>",
    "TreatScriptNamePatternMismatchAsError": "<Value>",
    "TreatHashMismatchAsError": "<Value>"
  },

  "PostScripts": {
    // ...
  },

  "ReferenceDataScripts": {
    // ...
  }
}
```

| Configuration | Description |
| ------------- | ----------- |
| ScriptBaseDirectory | The base directory where to store the target script type.
| ScriptNamePattern | The naming convention pattern used to validate and determination of execution order. 
| ScriptRecursiveSearch | Indicates whether scripts are searched recursively from the base directory
| GenerateScriptPath | Path to the generated scripts
| ExecutionFilterMode | The execution filter mode defines the strategy to use for execution order
| ExecutionFilterValue | The corresponding value based on the selected filter mode
| TreatScriptNamePatternMismatchAsError | Indicates how pattern mismatches should be handled. Throws an error if enabled and any mismatch is detected
| TreatHashMismatchAsError | If set to true any script change of already executed scripts will throw an error (hash based calculation). This rule does not apply to reference data scripts.

### Pre Configurations

|           | Default Value | Options |
|-----------|---------------|-----------------|
| ScriptBaseDirectory | {root}\Scripts\PreScripts | - |
| ScriptName Pattern | ^(\d{14})_(.*).sql | - |
| ScriptRecursiveSearch | true | *true* \| *false*
| GeneratedScriptPath | {root}\Scripts\RunPreScriptsGenerated.sql | - |
| ExecutionFilterMode |  all | *all* \| *count* \| *days* \| *date*
| ExecutionFilterValue | null
| TreatScriptNamePatternMismatchAsError | true | *true* \| *false*
| TreatHashMismatchAsError | true | *true* \| *false*

### Post Configurations

|           | Default Value | Options |
|-----------|---------------|-----------------|
| ScriptBaseDirectory | {root}\Scripts\PostScripts | - |
| ScriptName Pattern | ^(\d{14})_(.*).sql | - |
| ScriptRecursiveSearch | true | *true* \| *false*
| GeneratedScriptPath | {root}\Scripts\RunPostScriptsGenerated.sql | - |
| ExecutionFilterMode |  all | *all* \| *count* \| *days* \| *date*
| ExecutionFilterValue | null
| TreatScriptNamePatternMismatchAsError | true | *true* \| *false*
| TreatHashMismatchAsError | true | *true* \| *false*

### Reference Data Configurations

|           | Default Value | Options |
|-----------|---------------|-----------------|
| ScriptBaseDirectory | {root}\Scripts\ReferenceDataScripts | - |
| ScriptName Pattern | ^(\d+)_(.*).sql | - |
| ScriptRecursiveSearch | true | *true* \| *false*
| GeneratedScriptPath | {root}\Scripts\RunReferenceDataScriptsGenerated.sql | - |
| ExecutionFilterMode |  all | *all* \| *count* \| *days* \| *date*
| ExecutionFilterValue | null
| TreatScriptNamePatternMismatchAsError | true | *true* \| *false*
| TreatHashMismatchAsError | true | *true* \| *false*

## About
This extension has been developed by consultants of [4tecture](https://www.4tecture.ch) based on their experience from many DevOps projects. Originally it was developed internally without public scope. However, we decided to make this open source so others can benefit, too. 

Feedback is very welcome. Please open an issue on GitHub or send us a message through our website.

