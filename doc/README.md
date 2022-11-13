## Build with gradle

- To build the documentation execute `$ gradle asciidoctor`.
- The result can be found in the `./build/asciidoc/html5` directory.

## Build with docker

- Pull the image asciidoctor/docker-asciidoctor
  - `docker pull asciidoctor/docker-asciidoctor`
- Build the documentation
  - `docker run --rm -v %cd%\src\docs\asciidoc\:/documents/ asciidoctor/docker-asciidoctor asciidoctor-pdf ./index.adoc`
