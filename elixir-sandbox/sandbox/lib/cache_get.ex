defmodule CacheGetResponse do
#  @enforce_keys [:status]
#  defstruct [:status]

  defmodule Hit do
    @enforce_keys [:value_bytes]
    defstruct [:value_bytes]

    @type t :: %__MODULE__{
               value_bytes: binary()
     }

  #
#    @type t :: %__MODULE__{
#                 status: status(),
#                 players: {Player.t(), Player.t()} | nil,
#                 winning_player: Player.t() | nil
#               }

    @spec value_string(Hit.t) :: {String.t}
    def value_string(hit) do
      hit.value_bytes
    end

    @spec taco(String.t) :: String.t
    def taco(foo) do
      nil
    end
  end

#  @type status :: {:hit, Hit.t()} | :miss | :error

  @type cache_get_response :: {:hit, Hit.t()} | :miss | :error

end
