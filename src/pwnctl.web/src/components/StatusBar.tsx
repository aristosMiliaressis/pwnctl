import React, { Component } from 'react';

export class StatusBar extends Component<{}, {}>
{
    render() {
        return <div className="statusBar">Queued: 147 | Running: 4 | Finished: 836</div>
    }
}