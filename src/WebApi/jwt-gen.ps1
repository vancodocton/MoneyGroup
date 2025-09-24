param(
    [Parameter(Mandatory = $true)]
    [string]$Email
)

dotnet user-jwts create -o token --scheme Google --claim email_verified=true --claim email=$Email