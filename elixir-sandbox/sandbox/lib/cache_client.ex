defmodule CacheClient do
  defstruct name: 'foo'

  @type t :: %__MODULE__{
               name: String.t()
  }

  def create() do
    %CacheClient{name: 'bar'}
  end

  @spec get(CacheClient.t, String.t, String.t) :: CacheGetResponse.cache_get_response
  def get(%CacheClient{} = _client, _cache_name, _cache_key) do
#    {:hit, %CacheGetResponse.Hit{value_bytes: {:blech, :blargh}}}
#     {:hit, %CacheGetResponse.Hit{value_bytes: "taco taco man"}}
#    :miss
    :taco
  end
end
