## Build with gradle

- To build the documentation execute `$ gradle asciidoctor`.
- The result can be found in the `./build/asciidoc/html5` directory.

## Build with docker

- Pull the image asciidoctor/docker-asciidoctor
  - `docker pull asciidoctor/docker-asciidoctor`
- Start the docker container in interactive mode with a mounted volume
  - `docker run -it -v D:\FH\caas-bb-g2-buttinger-g2-emrah\doc\src\docs\asciidoc\:/documents/ asciidoctor/docker-asciidoctor`
- Build the documentation
  - `asciidoctor-pdf <PATH_TO_ASCIIDOC_FILE>`

