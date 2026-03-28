# Grave Shift
VR CoD Zombies mode with Superhot graphics and gravitational manipluation abilities
Target device: Meta Quest 3

Current target version: MVP (0.1)

Use the Unity MCP server & skill whenever necessary: UnityMCP: http://127.0.0.1:8080/mcp (HTTP)

## Roadmap
### MVP:
No animations
Player:
100 Hit points
Death: reset of map
1 single working gun, including shooting & gun handling, Pistole, only one slot, inf ammo
Teleportieren mit time und distance limit
Eine Map
Zombies (w/ path finding)
With nodes if another room, by distance if close
Same size as player, a bit slower, standard path finding, hits you when close
Randomly spawn out of dedicated locations

### 0.2:
Gravity Manipulation:
Rotate map by 90 degrees, character ability
More guns (SMG, Shotgun, Sturmgewehr)
In specif rounds get the option to get other gun
More zombies (Big and slow, another small and weak but annoying)
Highscore system: get points when killing enemies, highest score saved & displayed
Round system:
Define different rounds, progressively more hitpoints & harder enemy types

### 0.3:
Easter eggs
Better Animations
Menu



0.4:
1 more map

# Unity MCP
Feel free to update this seaction with quirks and workarounds discovered during development.

## Useful tips
- When adding custom project scripts as components, use the assembly-qualified name: `"PlayerController, Assembly-CSharp"` — plain class names don't resolve for project scripts.
- `create_script` has an aggressive validator that false-flags methods called from `Update()` as "duplicates" and any code in `Update()` as "string concatenation". Workaround: write scripts directly to disk with the `Write` tool, then call `refresh_unity` to trigger reimport.
- Cannot save scenes during play mode — call `manage_editor(action='stop')` first.
- Use `unity_reflect(action='search', scope='project')` to verify a script class is compiled before trying to add it as a component.
- Use `batch_execute` for bulk operations (creating many GameObjects, assigning materials) — much faster than sequential calls.
- `manage_camera(capture_source='scene_view')` does not support `view_position`/`view_rotation` — use `view_target` (a GameObject name) to frame the shot instead.
- `validate_script` needs the `uri` parameter (not `path`).