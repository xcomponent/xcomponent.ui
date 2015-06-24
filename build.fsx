// include Fake lib
#r @"packages/FAKE/tools/FakeLib.dll"
#r "System.Xml.Linq"

open Fake
open Fake.RestorePackageHelper
open Fake.TaskRunnerHelper
open Fake.XMLHelper
open System
open System.Text.RegularExpressions
open System.IO
open System.Xml.Linq

// parameters and constants definition
let product = "xcomponent.ui"
let authors = [ "xcomponent team" ]
let copyright = "Copyright INVIVOO Software"
let company = "INVIVOO Software"
let description = "UI shared library based on syncfusion"
let tags = ["xcomponent";"ui";"syncfusion"]
let descriptionTestTools = "nunit helpers"
let tagsTestTools = ["xcomponent";"ui";"syncfusion";"nunit";"test"]

let releaseConfig = "release"
let debugConfig = "debug"
let defaultVersion = "1.0.0-Z1"

let uiNuspecFile = "./UI/XComponent.UI.nuspec"
let testNuspecFile = "./UI.TestTools/XComponent.UI.TestTools.nuspec"
let buildDir = "./build/"
let nugetDir = buildDir @@ "nuget"
let libDir = nugetDir @@ @"lib\net45\"
let checker = "./Tools/CSProjReferencesChecker.exe"
let configuration = getBuildParamOrDefault "config" releaseConfig
let version = getBuildParamOrDefault "version" defaultVersion
let timeoutExec = 20.0

// helper functions
        
let formatVersion (strVersion:string) =    
    let regex = new Regex("/[ZDCG]/")
    let typeVersion = Regex.Match(strVersion, "[ZDCG]").Value
    
    let splitVersion = strVersion.Split('-')    
    let majorVersion = splitVersion.[0]
    let buildVersion = splitVersion.[1]        
    let splitBuild = buildVersion.Split((typeVersion.[0])) 
      
    let extVersion = splitBuild.[1]
    let finalExtVersion = 
        match (Convert.ToInt32(extVersion)) with
        | x when x < 10 ->
            "000" + extVersion
        | x when x < 100 ->
            "0" + extVersion        
        | x when x < 1000 ->            
            "0" + extVersion         
        | x when x > 60000 ->
            "9"
        | _ -> extVersion
          
    let finalTypeVersion = 
        match (typeVersion) with
        | "Z" -> "1"
        | "D" -> "2"
        | "C" -> "3"
        | "G" -> "4"
    majorVersion + "." + finalTypeVersion + finalExtVersion

let formattedVersion = formatVersion version

let getMSBuildFn =
    let properties = [("Configuration", configuration);("AssemblyCompany", company);("AssemblyCopyright", copyright); ("VersionNumber", formattedVersion)]
    fun (outputPath:string) targets projects ->            
            MSBuild outputPath targets properties projects

let replaceTag filePath patternToReplace newValue =
    ReplaceInFile (fun t ->         
        (>=>) patternToReplace newValue t        
        ) filePath    

// Targets
Target "RestorePackages" (fun _ -> 
     "XComponent.Common.sln"
     |> RestoreMSSolutionPackages (fun p ->
         { p with
             OutputPath = "./packages"
             Retries = 4 })
 )

Target "Check" (fun _ ->
    trace "checking csproj..."    
    let result = ExecProcess (fun info ->
                    info.FileName <- checker
                    info.WorkingDirectory <- __SOURCE_DIRECTORY__ 
                    info.Arguments <- " --rootDirectory=\".\" --repair --ignoreCsProjAnalysis=\"XComponent\" --xcTargetsLocation=\"$(MSBuildExtensionsPath)\\XComponent\\XComponent\" --checkedSolutions=\"XComponent.Common.sln\" --frameworkVersion=\"v4.5\" "
                    ) 
                    (TimeSpan.FromMinutes timeoutExec)
  
    if result <> 0 then failwithf "checker returned with a non-zero exit code"    
)

Target "Clean" (fun _ ->
    trace "Cleaning..."
    CleanDir buildDir

    !! "./**/*.csproj"
    |> getMSBuildFn "" "Clean"
    |> Log "MSBuild clean Output: "
)

Target "Compile" (fun _ ->    
    trace ("Compiling XComponent UI version " + version + " with configuration " + configuration + "...")

    !! "XComponent.Common.sln"
    |> getMSBuildFn "" "Rebuild"
    |> Log "MSBuild build Output: "
)

Target "RunTests" (fun _ ->
    trace "Running tests..."
    !! ("./test/**/bin/" + configuration + "/*Test.dll")
    |> NUnit (fun p ->
          {p with
             Framework = "v4.5";
             StopOnError = true;
             DisableShadowCopy = true;
             OutputFile = "./TestResults.xml" })
)

Target "CreatePackage" (fun _ ->       
    let package nuspecFile csprojFile packageDescription packageTags libFiles =
        let removeDir dir = 
            let del _ = 
                DeleteDir dir
                not (directoryExists dir)
            runWithRetries del 3 |> ignore

        ensureDirectory nugetDir
        printfn "Creating nuget packages..."

        CleanDir nugetDir
        
        let projectDir = Path.GetDirectoryName nuspecFile  
        let project = Path.GetFileNameWithoutExtension nuspecFile      
        let compileDir = projectDir @@ @"bin" @@ configuration
        let packages = projectDir @@ "packages.config"
        let packageDependencies = if (fileExists packages) then (getDependencies packages) else []                

        let getGacReferences (csprojFile:string) =
            let document = XDocument.Load (new StreamReader(csprojFile,true))
            document.Descendants()
            |> Seq.filter(fun t -> t.Name.LocalName = "Reference" && ((t.Descendants() |> Seq.length) = 0))
            |> Seq.map(fun t -> { NugetFrameworkAssemblyReferences.FrameworkVersions  = ["net45"]; NugetFrameworkAssemblyReferences.AssemblyName = t.Attribute(XName.Get("Include")).Value})
            |> Seq.distinct
            |> Seq.toList

        let pack outputDir =
            NuGetHelper.NuGet
                (fun p ->
                    { p with
                        Description = packageDescription
                        Authors = authors
                        Copyright = copyright
                        Project =  project
                        Properties = ["Configuration", configuration]                        
                        Version = formattedVersion
                        Tags = packageTags |> String.concat " "
                        FrameworkAssemblies = getGacReferences csprojFile
                        OutputPath = outputDir
                        WorkingDir = nugetDir                                                                   
                        Dependencies = packageDependencies })
                nuspecFile        

        // Copy dll, pdb and xml to libdir
        ensureDirectory libDir
        libFiles
        |> Seq.iter (fun f ->            
                CopyFileWithSubfolder compileDir libDir f)

        CopyFiles nugetDir ["LICENSE"; "README.md"]

        // Create both normal nuget package. 
        // Uses the files we copied to libDir and outputs to buildDir
        pack buildDir            
        
        removeDir nugetDir

    !! ("./UI/bin/"+ configuration + "/**/XComponent.*.dll")
    ++ ("./UI/bin/"+ configuration + "/XComponent.*.pdb")
    ++ ("./UI/bin/"+ configuration + "/XComponent.*.xml")
    ++ ("./UI/bin/"+ configuration + "/*AcroPDFLib.dll")    
    -- ("./UI/bin/"+ configuration + "/*CodeAnalysisLog*.xml")
    |> package uiNuspecFile "UI/XComponent.Common.UI.csproj" description tags
    
    !! ("./UI.TestTools/bin/" + configuration + "/XComponent.*.dll" )    
    ++ ("./UI.TestTools/bin/" + configuration + "/XComponent.*.pdb" )  
    ++ ("./UI.TestTools/bin/" + configuration + "/XComponent.*.xml" )      
    -- ("./UI.TestTools/bin/" + configuration + "/*CodeAnalysisLog*.xml" )    
    |> package testNuspecFile "UI.TestTools/XComponent.UI.TestTools.csproj" descriptionTestTools tagsTestTools
)
  
// Default target
Target "Help" (fun _ ->
    List.iter printfn [
      "XComponent.UI build usage: "      
      ""
      "build All   config=<config> "
      "            version=<version> "      
      ""
      "Arguments for ALL target:"
      "   config=<config>            build configuration: debug/release"      
      ""
      "   version=<version>          package version fased on xcomponent format"
      "                              format: x.y.y-[Z-C-G]build_number"
      "                              Z=dev / C=release candidate / G=release"
      "                              default value: 1.0.0-Z1"
      "compile, test and create nuget packages."      
      ""
      ""
      "build Compile   config=<config> "
      "                version=<version> " 
      "Just compile xcomponent ui."           
      ""
      ""      
      "Examples:"
      "  build All config=release version=1.2.0-G4"      
      ""]
)

Target "All" DoNothing

// Dependencies
"RestorePackages"
  ==> "Clean" 
  ==> "Check"  
  ==> "Compile"  
  ==> "RunTests"
  ==> "CreatePackage"
  ==> "All"

// start build
RunTargetOrDefault "Help"