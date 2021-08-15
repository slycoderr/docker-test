#!/bin/bash

set -e
run_cmd="dotnet run"

until mysql --version; do
>&2 echo "mysql is starting up"
sleep 1
done

>&2 echo "mysql is up - executing command"
exec $run_cmd