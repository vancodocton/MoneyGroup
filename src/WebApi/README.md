# MoneyGroup.WebApi

# Testing without Google OIDC
To test the Web API without Google OIDC, use `dotnet user-jwts` to issue new JWT.
```bash
dotnet user-jwts create -o token --scheme Bearer --claim email_verified=true --claim email=duongtruong.nguuyenky@gmail.com
```
Google issued JWTs will be not valid if run the above command. You can clear them by running:

```bash
dotnet user-jwts clear --force
```
