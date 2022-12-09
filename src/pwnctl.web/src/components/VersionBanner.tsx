import React, { Component } from 'react';

export class VersionBanner extends Component<{ version: string}>
{
    render() {
        return <div className="versionBanner">{this.props.version}</div>
    }
}