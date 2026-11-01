let base = ($env.USERPROFILE | path join ".glzr/glazewm")
let config = ($base | path join "config.yaml")

open $config
| merge deep {
    gaps: {
      outer_gap: {
        top: "56px"
        bottom: "58px"
        left: "68px"
        right: "68px"
      }
    }
  }
| to yaml
| save -f $config

^glazewm command wm-reload-config
