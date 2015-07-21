# Install / Build XComponent UI

XComponent.UI has an official [NuGet package](https://www.nuget.org/packages/XComponent.UI).

To install XComponent UI, run the following command in the Package Manager Console:

````
   PM> Install-Package XComponent.UI -Pre
````

And if you need unit testing helpers:

```
PM> Install-Package XComponent.UI.TestTools -Pre
```

You can also build it locally from the source code.

## Building XComponent.UI with Fake

The build as been ported to [Fake](http://fsharp.github.io/FAKE/) to make it
even easier to compile.

Clone the source code from GitHub:

````
    git clone https://github.com/Invivoo-software/xcomponent.ui
````

## Running build task

There is no need to install anything specific before running the build.

Once in the directory, run the build.cmd with the target All:

````
     build All
````

The ```All``` targets runs the following targets in order:

* Build
* Test
* Create Nuget package

### Running tests

To run unit tests from the command line, run the following command:

````
    build RunTests
````

### Creating Nuget distributions

To create and publish nuget packages locally, specify the package version and the nuget key:

````
    build PublishPackage config=Release version=<version> nugetkey=<key>
`````