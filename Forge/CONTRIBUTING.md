
# Build Artifacts

This configuration file is used by the project to define custom output directories for build artifacts.
 
## How to use
1. Copy `Directory.Build.props.sample` to your solution root and rename it to `Directory.Build.props`
2. Modify the `ForgePackageOutput` value to match your local unity project structure.
3. The `ForgePackageEditorOutput` is defined relative to `ForgePackageOutput` no changes needed.
4. Add `Directory.Build.props` to your .gitignore so that personal settings aren’t committed.

```xml
<?xml version="1.0" encoding="utf-8"?>
<Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <PropertyGroup>
    <!-- Base output directory for the com.kuru.forge package -->
    <ForgePackageOutput>D:\Engines\UnityProjects\com.kuru.forge.dev\Packages\com.kuru.forge\</ForgePackageOutput>
    <ForgePackageEditorOutput>$(ForgePackageOutput)\Editor</ForgePackageEditorOutput>
  </PropertyGroup>
</Project>
```