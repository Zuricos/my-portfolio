# my-portfolio
Personal finance portfolio platform for tracking investments across multiple brokers and institutions.

## Architecture

### Single-Tenant Mode
The application currently operates in single-tenant mode until user authentication is implemented. All data is associated with a placeholder user identified by `Guid.Empty` (`00000000-0000-0000-0000-000000000000`).

**Key Points:**
- All `Portfolio`, `Account`, `Activity`, and `Asset` entities default to `UserId = Guid.Empty`
- This is enforced via `ConstValues.DefaultUserId` constant
- When multi-user support is added, existing data will need migration to proper user accounts
- No authentication or authorization is currently implemented

### Domain Model
- **Portfolio**: Top-level aggregate representing a financial institution (broker, bank)
- **Account**: Sub-entity within portfolios for different account types (cash, securities, crypto)
- **Activity**: Individual transactions supporting both cash flows and asset trades
- **Asset**: Tradeable instruments with price history from external data providers

### Currency Support
- Portfolios have a default display currency
- Accounts can override the display currency
- Activities support multi-currency transactions via FX fields
- All currency codes must be ISO 4217 format (3-letter uppercase)

## Dependencies
Kudos goes to the following packages

### [Finance.NET](https://github.com/thorstenalpers/Finance.NET)
Copyright (c) 2024 by Thorsten
Licensed under the MIT License. See the [LICENSE](https://github.com/thorstenalpers/Finance.NET/blob/main/LICENSE) file in the repository for details.

### [CoinGecko.NET](https://github.com/JKorf/CoinGecko.Net)
Copyright (c) 2018 JKorf
Licensed under the MIT License. See the [LICENSE](https://github.com/JKorf/CoinGecko.Net/blob/main/LICENSE) file in the repository for details.
