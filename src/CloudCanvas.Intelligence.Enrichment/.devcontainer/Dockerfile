FROM ghcr.io/shibayan/devcontainers/azure-functions-python-base:3.11-noble

USER root

COPY .devcontainer/openssl/cosmos-emulator.crt /usr/local/share/ca-certificates/cosmos-emulator.crt
RUN apt-get update \
    && apt-get install -y --no-install-recommends ca-certificates curl sudo \
    && rm -rf /var/lib/apt/lists/* \
    && update-ca-certificates

USER vscode