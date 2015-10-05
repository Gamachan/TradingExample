## Workflow
1. Fork this repository
2. Create a *src* folder to contain your code
3. In the *src* directory, please create a Java or C# application that fulfills the [Application Specs](#application-specs)
4. Commit and push your code to your new repository, submit a pull request
5. We will review your submission and get back to you

## Application Specs
* Parse the "orderbook" file into timestamped OrderBook objects (hint: timestamps can be retrieved from the "message" file). This represents the most recent state of the market's visible order book (the "market data"); total order quantity at each price levels
 * Design the OrderBook class as you see fit
* Upon processing each OrderBook, perform the following actions:
 * Submit a new order (NewOrderSingle), if one is not already outstanding (a NewOrderSingle that was previously submitted and not yet cancelled), with a quantity of 100 to join any price level in the most recent OrderBook where the visible quantity is greater than 500
 * Submit a cancel (with an OrderCancel) for any outstanding orders if they are at a price level which no longer has a visible quantity of at least 200
* To send/submit an order, we will just "fake" it: create the object, serialize it (however you like) and send it out a TCP socket




