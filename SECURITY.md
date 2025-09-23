# Security Configuration

This document outlines the security configurations and best practices implemented in MoneyGroup.

## Environment Variables

The application uses environment variables for sensitive configuration values:

### Required Environment Variables

- `DB_PASSWORD`: Password for SQL Server database connection
- `POSTGRES_PASSWORD`: Password for PostgreSQL database connection

### Optional Environment Variables (for testing)

- `Test__Google__ClientId`: Google OAuth client ID for functional tests
- `Test__Google__ClientSecret`: Google OAuth client secret for functional tests
- `Test__Google__RefreshToken`: Google OAuth refresh token for functional tests

## Security Best Practices Implemented

1. **Removed hardcoded passwords** from configuration files
2. **Environment variable substitution** for sensitive data
3. **Conditional sensitive data logging** - only enabled in DEBUG builds
4. **Proper null checks** for configuration values
5. **Secure JSON encoder configuration** - avoiding null encoder setting

## Development Setup

1. Copy `.env.example` to `.env`
2. Update the values in `.env` with your actual credentials
3. The `.env` file is gitignored to prevent accidental commits

## Docker Configuration

The docker-compose files have been updated to use environment variables with fallback defaults for development.

## Notes

- Always use environment variables for passwords in production
- Never commit actual passwords to version control
- The application will fail to start if required environment variables are missing