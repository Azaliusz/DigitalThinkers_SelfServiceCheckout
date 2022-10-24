# DigitalThinkers_SelfServiceCheckout

This project implements the features for the following specification: [DigitalThinkers_SelfServiceCheckout_NET.pdf](./Docs/DigitalThinkers_SelfServiceCheckout_NET.pdf)

## Application profiles

The application contains two profiles:

- `SelfServiceCheckout_InMemory`
- `SelfServiceCheckout_SQLite`

In both cases, the application uses the Entity Framework for data management. In the case of `SelfServiceCheckout_InMemory`, the program can be run immediately after building. In the case of SelfServiceCheckout_SQLite, the following command must be run in the Package Manager Console after building:

```
Add-Migration InitialCreate -Context SelfServiceCheckoutSQLiteDbContext
```

This will create a SQLite database file under the following directory: [Data/Database](./SelfServiceCheckout/SelfServiceCheckout/Data/Database).
The db file name will be `SelfServiceCheckout.db`.

After run the api available on: http://localhost:5041

SwaggerUI: http://localhost:5041/swagger/index.html

## Constas

The constants needed to run the program can be found in the [appsettins.json](./SelfServiceCheckout/SelfServiceCheckout/appsettings.json) file.

The `MoneyOptions` section contains three configurable value:

- `DefaultCurrency`: one of the value of `Currencies` enum as string.
  - Currently accepted values: `HUF`, `EUR`
  - Default value: `HUF`
- `AcceptableDenominations`: An object of currency and denomination pairs, where the key value one of the value of `Currencies` enum as string and the value is a number array which specifies the acceptable denominations in the given currency.
- `CurrencyValueInDefaultCurrency`: An object of currency and value pairs, where the value specified for the currency is the value relative to the `DefaultCurrency`.

# Endpoints

## `/api/v1/Stock`

### Post method

This endpoint accept a json object denomination and count pairs. The denomination value must be listed in the `DefaultCurrency` array in the `AcceptableDenominations` otherwise, the following error message is received as a response:

```
The given denomination {{WRONG_DENOMINATION}} is not acceptable.
```

The quantity must be a positive number for every given denomination otherwise, the following error message will be received as a response:

```
"The amount specified for denomination {{DENOMINATION}} is invalid: {{WRONG_QUANTITY}}."
```

Example for an acceptable request body:

```json
{
  "1000": 3,
  "500": 1
}
```

If the request is acceptable then the aplication will store the given amounts of denominations and return the currently stored denominations from the `DefaultCurrency`.

### Get method

Returns the currently stored denominations from the `DefaultCurrency` in the following format:

```json
{
  "50": 3,
  "100": 20,
  "1000": 3
}
```

## `/api/v1/Checkout`

### Post method

This endpoint accept a json object with a price (`price`) value and a denomination, amount pairs (`inserted`) in the same format and restrictions as showed in the `/api/v1/Stock Post` method. Optionally, the currency of the denominations can be specified (`currency`).
The price value can not be negative number otherwise, the following error message will be received as a response:

```
The checkout price can not be negative value: {{WRONG_PRICE}}.
```

If the sum of the inserted denomination is less than the price, then the following error message will be received as a response:

```
The total value of coins given {{TOTAL_INSERTED_VALUE}} does not cover the price {{PRICE}}.
```

If the given currency is not the `DefaultCurrency` then the algoritms recalculates the total inserted value with the appropriate `CurrencyValueInDefaultCurrency` value.

Then determines the difference between the entered total value and the price and then rounds the resulting value according to the HUF rounding rules. The algorithms then try to return this value based on the denominations stored in the machine so far. If this is not possible, the following message comes as a response:

```
The machine cannot return {{VALUE}} based on the denominations stored in it.
```

If the return is possible, it updates the amount of stored denominations depending on the denominations entered and returned. If the entered currency was not the `DefaultCurrency` but the return was successful, the entered denominations will be also stored.
