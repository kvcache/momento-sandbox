defmodule SandboxTest do
  use ExUnit.Case
  doctest Sandbox

  test "greets the world" do
    assert Sandbox.hello() == :world

    client = CacheClient.create()
    get_response = CacheClient.get(client, "my-cache", "foo")
    IO.inspect(get_response)
    case get_response do
      {:hit, h} ->
        IO.puts("GOT A HIT")
        IO.inspect(h)
        IO.puts("VALUE STRING: #{CacheGetResponse.Hit.value_string(h)}")

      :miss ->
        IO.puts("MISS")

      :taco ->
        "what a world"
    end

    is_binary(42)
    CacheGetResponse.Hit.value_string(42)

#    get_response = %CacheGetResponse{status: {:hit, "taco"}}
#    IO.puts "WTF"
#    IO.inspect get_response
  end
end
