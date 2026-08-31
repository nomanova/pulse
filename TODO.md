# TODO

Create a one-off sending of a message.

## Plugins

- Endpoints to remove/fetch/search connections
- Support for loading multiple versions of the same plugin
- Validation of plugin id and version format (semver)

## Workflows

- Unit tests for domain logic
- Option to fetch the current draft and published versions => generalize to worfklow version search endpoint
- Endpoint to publish a workflow
- Data plane endpoint to trigger a workflow

- Change the order of a step in a (draft) workflow

- Submit all steps of a workflow using JSON or YAML

- Allow branching / parallel execution of workflow steps

## Connections

- Update endpoint
- Search/fetch endpoints

## CLI Tool

- Improved error message when token expired
- Confirmation on delete
- Sanitize server url
- When removing a server/organization/application/environment and only one is left, auto-select it

## Others

- Renaming of org, app, env, workflow entities