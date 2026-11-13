let base = ($env.USERPROFILE | path join ".glzr/glazewm")
let config = ($base | path join "config.yaml")

open $config
| merge deep {
    gaps: {
      outer_gap: {
        top: "25px"
        bottom: "13px"
        right: "10px"
        left: "10px"
      }
    }
  }
| to yaml
| save -f $config

^glazewm command wm-reload-config
