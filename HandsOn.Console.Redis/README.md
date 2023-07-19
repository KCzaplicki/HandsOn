# HandsOn.Console.Redis

This project provides a hands-on experience for testing various functionalities of Redis, including cache, counter, and pub/sub. It aims to demonstrate the practical implementation and usage of Redis.

## Setup

Example use docker to setup Redis. Please run `docker compose up` before run examples.

## Redis features
Redis offers the following functionalities:
- Key-Value Store
- Caching
- Pub/Sub Messaging
- Transactions
- Distributed locks
- Data structures i.e. Counter

## Projects
|Name|Description|
|-|-|
|HandsOn.Console.Redis|Project testing set and retrieval key/value data in Redis, as well as checking the status of the store|
|HandsOn.Console.Redis.Tests|Project testing more advanced scenarios such as counters, transactions and caching|
|HandsOn.Console.Redis.Publisher|project testing message publishing using Redis pub/sub with literal and pattern channels|
|HandsOn.Console.Redis.Subscriber|project testing message subscribing using Redis pub/sub with literal and pattern channels|
|HandsOn.Console.Redis.ServiceA|Project testing distributed locks in Redis|
|HandsOn.Console.Redis..ServiceB|Project testing distributed locks in Redis|

## Libraries
C# Wrappers for Redis

- [StackExchange.Redis](https://github.com/StackExchange/StackExchange.Redis)
- [RedLock.net](https://github.com/samcook/RedLock.net)

## Links
- [StackExchange.Redis docs](https://stackexchange.github.io/StackExchange.Redis/) - documentation with quick start for redis
- [RedLock.net github](https://github.com/samcook/RedLock.net) - Github page for redlock.net for distributed locking with redis
- [Redis - getting started](https://redis.io/docs/getting-started/install-stack/docker/) - How to install Redis using Docker