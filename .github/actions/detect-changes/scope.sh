#!/usr/bin/env bash
# Chooses what to compare this run against and writes it to GITHUB_OUTPUT:
#   mode=filter  compare paths (against `base` when it is set)
#   mode=full    run everything, because no safe comparison exists
#   mode=none    run nothing, because a successful run already covers this commit
set -euo pipefail

mode=full
base=""

case "${EVENT}" in
  pull_request)
    mode=filter
    ;;
  push)
    # Comparing against the previous commit would lose changes: a newer run
    # replaces a pending one in the concurrency group, so a commit can end up
    # with no run at all. The last successful run has no such gap.
    last_success="$(gh run list \
      --workflow "${WORKFLOW}" \
      --branch "${BRANCH}" \
      --status success \
      --limit 20 \
      --json headSha,event \
      --jq '[.[] | select(.event != "pull_request")][0].headSha // ""')" || {
      echo "::warning::Could not read run history, so running everything."
      last_success=""
    }

    if [[ -z "${last_success}" ]] || ! git cat-file -e "${last_success}^{commit}" 2>/dev/null; then
      mode=full
    elif git merge-base --is-ancestor "${GITHUB_SHA}" "${last_success}"; then
      # Includes re-running an older run, which must not redeploy older code.
      mode=none
    elif git merge-base --is-ancestor "${last_success}" "${GITHUB_SHA}"; then
      mode=filter
      base="${last_success}"
    fi
    ;;
esac

echo "Comparison: mode=${mode} base=${base:-<none>}"
{
  echo "mode=${mode}"
  echo "base=${base}"
} >> "${GITHUB_OUTPUT}"
