package main

import (
	"encoding/json"
	"fmt"
	"io"
	"os"
)

func main() {
	if err := run(); err != nil {
		fmt.Fprintln(os.Stderr, err)
		os.Exit(1)
	}
}
func run() error {
	data, err := io.ReadAll(os.Stdin)
	if err != nil {
		return err
	}
	input, err := decodeInput(data)
	if err != nil {
		return err
	}
	output, err := runSchool(input.History, input.Operations)
	if err != nil {
		return err
	}
	return json.NewEncoder(os.Stdout).Encode(output)
}
