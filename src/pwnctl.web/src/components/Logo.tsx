import React, { Component } from 'react';

export class Logo extends Component<{logo: string}>
{
    render() {
        return (
            <div style={{ paddingLeft: '40px', float: 'left' }}>
                <img src={this.props.logo} height="75px" alt="logo" />
            </div>);
    }
}