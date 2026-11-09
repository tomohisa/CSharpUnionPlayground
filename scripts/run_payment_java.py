#!/usr/bin/env python3
"""Fetch pinned Jackson jars into a temporary directory and launch one Java source file."""
from pathlib import Path
import os
import subprocess
import tempfile
import urllib.request

def jackson_classpath(directory):
    jars = []
    for artifact, version in [('jackson-annotations', '2.23'), ('jackson-core', '2.23.0'), ('jackson-databind', '2.23.0')]:
        jar = Path(directory) / f'{artifact}-{version}.jar'
        url = f'https://repo.maven.apache.org/maven2/com/fasterxml/jackson/core/{artifact}/{version}/{jar.name}'
        urllib.request.urlretrieve(url, jar)
        jars.append(str(jar))
    return os.pathsep.join(jars)


if __name__ == '__main__':
    source = Path(__file__).resolve().parents[1] / 'samples/file-based/Payment.java'
    with tempfile.TemporaryDirectory(prefix='cupj-payment-java-') as directory:
        subprocess.run(['java', '--class-path', jackson_classpath(directory), str(source)], check=True)
