#!/usr/bin/env bash
set -e
root="$(cd "$(dirname "$0")" && pwd)"
trap 'kill 0' EXIT
(cd "$root/src/NotificationApi" && dotnet run) &
(cd "$root/src/OrderApi" && dotnet run) &
wait
