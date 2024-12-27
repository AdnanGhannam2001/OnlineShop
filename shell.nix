{ pkgs ? import <nixpkgs> {} }:

pkgs.mkShell {
  buildInputs = [
    pkgs.python3
    pkgs.python3Packages.faker
    pkgs.python3Packages.nanoid
    pkgs.dotnet-sdk_8
  ];
}
