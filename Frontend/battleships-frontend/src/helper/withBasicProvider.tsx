import React from "react";

export function withBasicProvider(Provider: any) {
  return (Component: React.FC) => (props: any) => (
    <Provider>
      <Component {...props} />
    </Provider>
  );
}