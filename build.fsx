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
let copyright = "Copyright XComponent"
let company = "XComponent"
let description = "UI shared library based on syncfusion"
let tags = ["xcomponent";"ui";"syncfusion"]
let descriptionTestTools = "nunit helpers"
let tagsTestTools = ["xcomponent";"ui";"syncfusion";"nunit";"test"]

let releaseConfig = "release"
let debugConfig = "debug"
let defaultVersion = "1.0.0-build1"

let uiNuspecFile = "./UI/XComponent.UI.nuspec"
let testNuspecFile = "./UI.TestTools/XComponent.UI.TestTools.nuspec"
let buildDir = "./build/"
let nugetDir = buildDir @@ "nuget"
let libDir = nugetDir @@ @"lib\net45\"
let checker = "./Tools/CSProjReferencesChecker.exe"
let configuration = getBuildParamOrDefault "config" releaseConfig
let version = getBuildParamOrDefault "version" defaultVersion
let nugetExe = FullName @"./Tools/NuGet.exe"
let timeoutExec = 20.0

// helper functions
        
let formatAssemblyVersion (strVersion:string) =        
    let typeVersionMatch = Regex.Match(strVersion, "build|rc")    
    match typeVersionMatch.Success with
    | true ->
        let typeVersion = typeVersionMatch.Value
        let splitVersion = strVersion.Split('-')   
    
        let majorVersion = splitVersion.[0]        
        let buildVersion = splitVersion.[1]                           

        let extVersion = buildVersion.Substring(typeVersion.Length, buildVersion.Length - typeVersion.Length)
        
        let finalExtVersion = 
            match (Convert.ToInt32(extVersion)) with
            | x when x < 10 ->
                "000" + extVersion
            | x when x < 100 ->
                "00" + extVersion        
            | x when x < 1000 ->            
                "0" + extVersion         
            | x when x > 60000 ->
                "9"
            | _ -> extVersion
          
        let finalTypeVersion = 
            match (typeVersion) with
            | "build" -> "1"
            | "rc" -> "2" 
            | _ -> "3"           
        majorVersion + "." + finalTypeVersion + finalExtVersion
    | false -> strVersion + ".4" // specific release number

let formatNugetVersion (strVersion:string) =        
    let typeVersionMatch = Regex.Match(strVersion, "build|rc")    
    match typeVersionMatch.Success with
    | true ->
        let typeVersion = typeVersionMatch.Value
        let splitVersion = strVersion.Split('-')   
    
        let majorVersion = splitVersion.[0]        
        let buildVersion = splitVersion.[1]                           

        let extVersion = buildVersion.Substring(typeVersion.Length, buildVersion.Length - typeVersion.Length)
        
        let finalExtVersion = 
            match (Convert.ToInt32(extVersion)) with
            | x when x < 10 ->
                "00" + extVersion
            | x when x < 100 ->
                "0" + extVersion        
            | x when x > 1000 ->            
                extVersion                     
            | _ -> extVersion
        
        majorVersion + "-" + typeVersion + "v" + finalExtVersion
    | false -> strVersion

let formattedAssemblyVersion = formatAssemblyVersion version

let formattedNugetVersion = formatNugetVersion  version

let getMSBuildFn =
    let properties = [("Configuration", configuration);("AssemblyCompany", company);("AssemblyCopyright", copyright); ("VersionNumber", formattedAssemblyVersion)]
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
        
        let projectDir = Path.GetDirectoryName nuspecFile  
        let project = Path.GetFileNameWithoutExtension nuspecFile      
        let compileDir = projectDir @@ @"bin" @@ configuration
        let packages = projectDir @@ "packages.config"
        let packageDependencies = if (fileExists packages) then (getDependencies packages) else []                

        let getGacReferences (csprojFile:string) =
            let document = XDocument.Load (new StreamReader(csprojFile,true))
            document.Descendants()
            |> Seq.filter(fun t -> t.Name.LocalName = "Reference")
            |> Seq.filter(fun t -> not (t.Descendants() |> Seq.exists(fun r -> r.Name.LocalName = "HintPath")))
            |> Seq.filter(fun t -> not(t.Descendants() |> Seq.exists(fun r -> r.Name.LocalName = "Private") || t.Descendants() |> Seq.exists(fun r -> r.Name.LocalName = "Private" && r.Value.ToLower() = "true" )))
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
                        Version = formattedNugetVersion
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

    !! ("./UI/bin/"+ configuration + "/**/XComponent.*.dll")
    ++ ("./UI/bin/"+ configuration + "/XComponent.*.pdb")
    ++ ("./UI/bin/"+ configuration + "/XComponent.*.xml")
    ++ ("./UI/bin/"+ configuration + "/*AcroPDFLib.dll")    
    ++ ("./UI/bin/"+ configuration + "/Syncfusion*.dll")    
    -- ("./UI/bin/"+ configuration + "/*CodeAnalysisLog*.xml")
    |> package uiNuspecFile "UI/XComponent.Common.UI.csproj" description tags
    
    !! ("./UI.TestTools/bin/" + configuration + "/XComponent.*.dll" )    
    ++ ("./UI.TestTools/bin/" + configuration + "/XComponent.*.pdb" )  
    ++ ("./UI.TestTools/bin/" + configuration + "/XComponent.*.xml" )      
    -- ("./UI.TestTools/bin/" + configuration + "/*CodeAnalysisLog*.xml" )    
    |> package testNuspecFile "UI.TestTools/XComponent.UI.TestTools.csproj" descriptionTestTools tagsTestTools
)

Target "PublishPackage" (fun _ ->    
    let publishNugetPackages _ = 
        let rec publishPackage accessKey trialsLeft packageFile =
            let tracing = enableProcessTracing
            enableProcessTracing <- false
            let args pack key = sprintf "push -Source https://www.nuget.org/api/v2/package \"%s\" %s" pack key                

            tracefn "Pushing %s Attempts left: %d" (FullName packageFile) trialsLeft
            try 
                let result = ExecProcess (fun info -> 
                        info.FileName <- nugetExe
                        info.WorkingDirectory <- (Path.GetDirectoryName (FullName packageFile))
                        info.Arguments <- (args packageFile accessKey)) (System.TimeSpan.FromMinutes 1.0)
                enableProcessTracing <- tracing
                if result <> 0 then failwithf "Error during NuGet symbol push. %s %s" nugetExe (args packageFile accessKey)
            with exn -> 
                if (trialsLeft > 0) then (publishPackage accessKey (trialsLeft-1) packageFile)
                else raise exn
        printfn "Pushing nuget packages"
        let normalPackages= 
            !! (buildDir @@ "*.nupkg") 
            |> Seq.sortBy(fun x -> x.ToLower())
        for package in normalPackages do
            publishPackage (getBuildParam "nugetkey") 3 package
            
    publishNugetPackages()
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
      "                              dev and rc format: x.y.z-[build|rc]build_number"
      "                              release format: x.y.z "
      "                              default value: 1.0.0-dev1"
      "                              example: 3.2.1-rc3"
      "compile, test and create nuget packages."      
      ""
      ""
      "build Compile   config=<config> "
      "                version=<version> " 
      "Just compile xcomponent ui."           
      ""
      ""      
      "Examples:"
      "  build All config=release version=1.2.0-rc4"      
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

"All"
  ==> "PublishPackage"

// start build
RunTargetOrDefault "Help"