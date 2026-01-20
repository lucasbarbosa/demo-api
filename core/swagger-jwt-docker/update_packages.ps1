$projects = @(
    "src/DemoApi.Api/DemoApi.Api.csproj",
    "src/DemoApi.Application/DemoApi.Application.csproj",
    "tests/DemoApi.Api.Tests/DemoApi.Api.Tests.csproj",
    "tests/DemoApi.Application.Tests/DemoApi.Application.Tests.csproj",
    "tests/DemoApi.Tests.Builders/DemoApi.Tests.Builders.csproj"
)

# Update all packages in all projects
foreach ($proj in $projects) {
    Write-Host "Updating packages for $proj..."
    # Get list of packages
    $xml = [xml](Get-Content $proj)
    $packages = $xml.Project.ItemGroup.PackageReference
    foreach ($pkg in $packages) {
        $pkgName = $pkg.Include
        Write-Host "Updating $pkgName in $proj..."
        dotnet add $proj package $pkgName
    }
}
