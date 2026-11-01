let base = ($env.USERPROFILE | path join ".glzr/glazewm")
let config = ($base | path join "config.yaml")

open $config
| merge deep {
    gaps: {
      outer_gap: {
        top: "24px"
        bottom: "14px"
        right: "16px"
        left: "16px"
      }
    }
  }
| to yaml
| save -f $config

^glazewm command wm-reload-config
